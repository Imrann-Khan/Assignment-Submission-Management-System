"use client";

import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { setCookie, deleteCookie } from "@/lib/cookies";
import type { LoginResult } from "@/types/auth";

interface AuthUser {
  userId: string;
  fullName: string;
  email: string;
  role: "Admin" | "Teacher" | "Student";
}

interface AuthContextValue {
  user: AuthUser | null;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<AuthUser>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);
const STORAGE_KEY = "auth_user";

export function AuthProvider({ children }: { children: ReactNode }) {
  const router = useRouter();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored) {
      setUser(JSON.parse(stored));
    }
    setIsLoading(false);
  }, []);

  async function login(email: string, password: string) {
    const result = await api.post<LoginResult>("/api/auth/login", { email, password });

    const authUser: AuthUser = {
      userId: result.userId,
      fullName: result.fullName,
      email: result.email,
      role: result.role,
    };

    localStorage.setItem("token", result.token);
    localStorage.setItem(STORAGE_KEY, JSON.stringify(authUser));
    setCookie("role", authUser.role, 1);

    setUser(authUser);
    return authUser;
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem(STORAGE_KEY);
    deleteCookie("role");
    setUser(null);
    router.push("/login");
  }

  return (
    <AuthContext.Provider value={{ user, isLoading, login, logout }}>
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
