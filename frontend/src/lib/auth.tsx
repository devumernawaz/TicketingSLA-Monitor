"use client";

import { createContext, useContext, useState, useEffect, ReactNode } from "react";

export type UserRole = "Admin" | "Agent" | "Client";

export interface AuthUser {
  id: string;
  name: string;
  role: UserRole;
  tenantId: string;
}

interface AuthContextValue {
  user: AuthUser | null;
  login: (user: AuthUser) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

// TEMPORARY: mock users standing in for real JWT-based login.
// Swap this out entirely once real authentication exists — nothing
// outside this file needs to change when that happens.
const MOCK_USERS: Record<UserRole, AuthUser> = {
  Admin: { id: "admin-1", name: "Ayesha (Admin)", role: "Admin", tenantId: "11111111-1111-1111-1111-111111111111" },
  Agent: { id: "agent-1", name: "Bilal (Agent)", role: "Agent", tenantId: "11111111-1111-1111-1111-111111111111" },
  Client: { id: "client-1", name: "Sara (Client)", role: "Client", tenantId: "11111111-1111-1111-1111-111111111111" },
};

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);

  useEffect(() => {
    const match = document.cookie.match(/(?:^|; )mockRole=([^;]*)/);
    const role = match?.[1] as UserRole | undefined;
    if (role && MOCK_USERS[role]) {
      setUser(MOCK_USERS[role]);
    }
  }, []);

  const login = (u: AuthUser) => {
    setUser(u);
    document.cookie = `mockRole=${u.role}; path=/`;
  };

  const logout = () => {
    setUser(null);
    document.cookie = "mockRole=; path=/; max-age=0";
  };

  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
}

export { MOCK_USERS };