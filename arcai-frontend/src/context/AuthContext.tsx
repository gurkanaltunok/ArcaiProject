import React, { createContext, useContext, useState, useEffect, ReactNode } from 'react';
import { User } from '../types';
import apiService from '../services/apiService';

interface AuthContextType {
  isAuthenticated: boolean;
  token: string | null;
  user: User | null;
  role: string | null;
  login: (token: string, user: User) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};

interface AuthProviderProps {
  children: ReactNode;
}

export const AuthProvider: React.FC<AuthProviderProps> = ({ children }) => {
  const [token, setToken] = useState<string | null>(localStorage.getItem('token'));
  const [user, setUser] = useState<User | null>(() => {
    const storedUser = localStorage.getItem('user');
    return storedUser ? JSON.parse(storedUser) : null;
  });
  const [role, setRole] = useState<string | null>(user?.role || null);

  // Fetch user information from API on page load
  useEffect(() => {
    const fetchCurrentUser = async () => {
      if (token) {
        try {
          const response = await apiService.get<User>('/auth/me');
          const currentUser = response.data;
          setUser(currentUser);
          setRole(currentUser.role);
          localStorage.setItem('user', JSON.stringify(currentUser));
        } catch (error) {
          console.error('Failed to fetch current user:', error);
          // Clear token on error
          if (error && typeof error === 'object' && 'response' in error && (error as any).response?.status === 401) {
            setToken(null);
            setUser(null);
            setRole(null);
            localStorage.removeItem('token');
            localStorage.removeItem('user');
          }
        }
      }
    };

    fetchCurrentUser();
  }, [token]);

  useEffect(() => {
    if (token && user) {
      // Token decode edilip role'ü çıkar (JWT token'dan)
      // Basit bir yaklaşım: token'ı parse et (gerçekte jwt-decode kullanılabilir)
      try {
        const tokenParts = token.split('.');
        if (tokenParts.length === 3) {
          const payload = JSON.parse(atob(tokenParts[1]));
          setRole(payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || user.role);
        }
      } catch (e) {
        setRole(user.role);
      }
    }
  }, [token, user]);

  const login = (newToken: string, newUser: User) => {
    setToken(newToken);
    setUser(newUser);
    setRole(newUser.role);
    localStorage.setItem('token', newToken);
    localStorage.setItem('user', JSON.stringify(newUser));
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    setRole(null);
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  };

  const value: AuthContextType = {
    isAuthenticated: !!token,
    token,
    user,
    role,
    login,
    logout,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

