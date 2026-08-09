"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";
import { NotificationBell } from "@/components/NotificationBell";
import { BrandMark } from "@/components/BrandMark";

export default function TeacherLayout({ children }: { children: React.ReactNode }) {
  const { user, logout } = useAuth();
  const pathname = usePathname();

  return (
    <div className="min-h-screen bg-gray-50">
      <header className="border-b border-gray-200 bg-white shadow-sm">
        <div className="mx-auto flex max-w-6xl flex-col gap-3 px-4 py-3 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-wrap items-center gap-x-6 gap-y-2">
            <div className="flex items-center gap-2">
              <BrandMark />
              <span className="font-semibold text-gray-900">Teacher Panel</span>
            </div>
            <Link
              href="/teacher"
              className={`text-sm transition-colors ${
                pathname === "/teacher" ? "font-semibold text-blue-600" : "text-gray-600 hover:text-gray-900"
              }`}
            >
              My Assignments
            </Link>
          </div>
          <div className="flex items-center gap-4">
            <NotificationBell />
            <span className="text-sm text-gray-600">{user?.fullName}</span>
            <button onClick={logout} className="text-sm text-red-600 transition-colors hover:underline">
              Logout
            </button>
          </div>
        </div>
      </header>
      <main className="mx-auto max-w-6xl px-4 py-8">{children}</main>
    </div>
  );
}
