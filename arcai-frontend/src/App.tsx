import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Home from './pages/Home';
import MyRequests from './pages/MyRequests';
import Profile from './pages/Profile';
import AdminUsers from './pages/admin/AdminUsers';
import AdminDocuments from './pages/admin/AdminDocuments';
import AdminRequests from './pages/admin/AdminRequests';
import AdminBorrowingHistory from './pages/admin/AdminBorrowingHistory';
import AdminSettings from './pages/admin/AdminSettings';

const AppRoutes: React.FC = () => {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={isAuthenticated ? <Navigate to="/" replace /> : <Login />} />
      
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<Home />} />
        <Route path="my-requests" element={<MyRequests />} />
        <Route path="profile" element={<Profile />} />

        <Route
          path="admin/*"
          element={
            <AdminRoute>
              <Routes>
                <Route path="users" element={<AdminUsers />} />
                <Route path="documents" element={<AdminDocuments />} />
                <Route path="requests" element={<AdminRequests />} />
                <Route path="borrowing-history" element={<AdminBorrowingHistory />} />
                <Route path="settings" element={<AdminSettings />} />
              </Routes>
            </AdminRoute>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AppRoutes />
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;
