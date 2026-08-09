"use client";

import { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import { api } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import { useAuth } from "@/contexts/AuthContext";
import { formatDateTime } from "@/lib/format";
import type { NotificationDto } from "@/types/notification";
import type { PagedResult } from "@/types/pagination";

export function NotificationBell() {
  const { user } = useAuth();
  const router = useRouter();
  const [isOpen, setIsOpen] = useState(false);
  const containerRef = useRef<HTMLDivElement>(null);

  const { data, refetch } = useApiQuery<PagedResult<NotificationDto>>("/api/notifications?pageSize=10");
  const { data: unreadCount, refetch: refetchUnreadCount } = useApiQuery<number>("/api/notifications/unread-count");
  const notifications = data?.items ?? [];

  useEffect(() => {
    function handleClickOutside(event: MouseEvent) {
      if (containerRef.current && !containerRef.current.contains(event.target as Node)) {
        setIsOpen(false);
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, []);

  function assignmentHref(assignmentId: string) {
    return user?.role === "Teacher" ? `/teacher/assignments/${assignmentId}` : `/student/assignments/${assignmentId}`;
  }

  async function handleNotificationClick(notification: NotificationDto) {
    setIsOpen(false);
    if (!notification.isRead) {
      await api.patch(`/api/notifications/${notification.id}/read`);
      refetch();
      refetchUnreadCount();
    }
    if (notification.relatedAssignmentId) {
      router.push(assignmentHref(notification.relatedAssignmentId));
    }
  }

  async function handleMarkAllAsRead() {
    await api.patch("/api/notifications/read-all");
    refetch();
    refetchUnreadCount();
  }

  return (
    <div ref={containerRef} className="relative">
      <button
        onClick={() => setIsOpen((open) => !open)}
        className="relative rounded-full p-2 text-gray-600 hover:bg-gray-100"
        aria-label="Notifications"
      >
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" fill="currentColor" className="h-5 w-5">
          <path
            fillRule="evenodd"
            d="M5.25 9a6.75 6.75 0 0113.5 0v.75c0 2.123.8 4.057 2.118 5.52a.75.75 0 01-.297 1.206c-1.544.57-3.16.99-4.831 1.243a3.75 3.75 0 11-7.48 0 24.585 24.585 0 01-4.831-1.244.75.75 0 01-.298-1.205A8.217 8.217 0 005.25 9.75V9zm4.502 8.9a2.25 2.25 0 104.496 0 25.057 25.057 0 01-4.496 0z"
            clipRule="evenodd"
          />
        </svg>
        {!!unreadCount && unreadCount > 0 && (
          <span className="absolute -right-0.5 -top-0.5 flex h-4 min-w-4 items-center justify-center rounded-full bg-red-600 px-1 text-[10px] font-medium text-white">
            {unreadCount > 9 ? "9+" : unreadCount}
          </span>
        )}
      </button>

      {isOpen && (
        <div className="absolute right-0 z-10 mt-2 w-80 max-w-[calc(100vw-2rem)] rounded-lg border border-gray-200 bg-white shadow-lg">
          <div className="flex items-center justify-between border-b border-gray-100 px-3 py-2">
            <span className="text-sm font-medium text-gray-900">Notifications</span>
            {!!unreadCount && unreadCount > 0 && (
              <button onClick={handleMarkAllAsRead} className="text-xs text-blue-600 hover:underline">
                Mark all as read
              </button>
            )}
          </div>
          <div className="max-h-96 overflow-y-auto">
            {notifications.length === 0 && (
              <p className="px-3 py-6 text-center text-sm text-gray-500">No notifications yet.</p>
            )}
            {notifications.map((n) => (
              <button
                key={n.id}
                onClick={() => handleNotificationClick(n)}
                className={`block w-full border-b border-gray-50 px-3 py-2.5 text-left text-sm hover:bg-gray-50 ${
                  n.isRead ? "text-gray-500" : "bg-blue-50/50 font-medium text-gray-900"
                }`}
              >
                <p>{n.message}</p>
                <p className="mt-0.5 text-xs text-gray-400">{formatDateTime(n.createdAt)}</p>
              </button>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
