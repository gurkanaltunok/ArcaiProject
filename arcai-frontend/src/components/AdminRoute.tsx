import React, { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import ProtectedRoute from './ProtectedRoute';

interface AdminRouteProps {
  children: ReactNode;
}

const AdminRoute: React.FC<AdminRouteProps> = ({ children }) => {
  const { role } = useAuth();

  return (
    <ProtectedRoute>
      {role === 'Admin' ? <>{children}</> : <Navigate to="/" replace />}
    </ProtectedRoute>
  );
};

export default AdminRoute;

