'use client';

import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { UserInfo } from '@/types/common.types';

interface AuthContextType {
  isAuthenticated: boolean;
  user: UserInfo | null;
  token: string | null;
  login: (token: string, refreshToken: string, user: UserInfo) => void;
  logout: () => void;
  hasPermission: (permission: string) => boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserInfo | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const savedToken = localStorage.getItem('token');
    const savedUser = localStorage.getItem('user');
    if (savedToken && savedUser) {
      setToken(savedToken);
      setUser(JSON.parse(savedUser));
    }
  }, []);

  const login = (newToken: string, refreshToken: string, newUser: UserInfo) => {
    localStorage.setItem('token', newToken);
    localStorage.setItem('refreshToken', refreshToken);
    localStorage.setItem('user', JSON.stringify(newUser));
    setToken(newToken);
    setUser(newUser);
  };

  const logout = () => {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    localStorage.removeItem('tenantId');
    setToken(null);
    setUser(null);
  };

  const hasPermission = (permission: string) => {
    return user?.permissions?.includes(permission) ?? false;
  };

  return (
    <AuthContext.Provider value={{ isAuthenticated: !!token, user, token, login, logout, hasPermission }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};
