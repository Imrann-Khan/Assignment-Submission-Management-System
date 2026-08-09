import Link from "next/link";
import type { ReactNode } from "react";

const iconProps = {
  viewBox: "0 0 24 24",
  fill: "none",
  stroke: "currentColor",
  strokeWidth: 1.6,
  strokeLinecap: "round" as const,
  strokeLinejoin: "round" as const,
  className: "h-5 w-5",
};

const sections: { href: string; title: string; description: string; icon: ReactNode }[] = [
  {
    href: "/admin/users",
    title: "Users",
    description: "Create and manage Admin, Teacher, and Student accounts.",
    icon: (
      <svg {...iconProps}>
        <circle cx="9" cy="8" r="2.75" />
        <circle cx="16" cy="9.5" r="2.1" />
        <path d="M4 19c0-3.3 2.2-5.5 5.5-5.5S15 15.7 15 19" />
        <path d="M14 14c2.4.4 4 2.3 4 5" />
      </svg>
    ),
  },
  {
    href: "/admin/classes",
    title: "Classes & Subjects",
    description: "Manage classes and the subjects taught within them.",
    icon: (
      <svg {...iconProps}>
        <path d="M4 6.5C4 5.7 4.7 5 5.5 5H11v14H5.5c-.8 0-1.5-.7-1.5-1.5v-11z" />
        <path d="M20 6.5c0-.8-.7-1.5-1.5-1.5H13v14h5.5c.8 0 1.5-.7 1.5-1.5v-11z" />
      </svg>
    ),
  },
  {
    href: "/admin/teacher-assignments",
    title: "Teacher Assignments",
    description: "Assign teachers to the subjects they teach.",
    icon: (
      <svg {...iconProps}>
        <path d="M7 7h10l-3-3" />
        <path d="M17 17H7l3 3" />
      </svg>
    ),
  },
  {
    href: "/admin/assignments",
    title: "Assignments",
    description: "Read-only view of every assignment across the system.",
    icon: (
      <svg {...iconProps}>
        <rect x="6" y="4" width="12" height="16" rx="2" />
        <path d="M9 9h6M9 13h6M9 17h3" />
      </svg>
    ),
  },
  {
    href: "/admin/submissions",
    title: "Submissions",
    description: "Read-only view of every submission across the system.",
    icon: (
      <svg {...iconProps}>
        <path d="M4 12h4l2 3h4l2-3h4" />
        <path d="M4 12V7a1 1 0 011-1h14a1 1 0 011 1v5" />
        <path d="M4 12v5a1 1 0 001 1h14a1 1 0 001-1v-5" />
      </svg>
    ),
  },
];

export default function AdminHomePage() {
  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">Admin Dashboard</h1>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
        {sections.map((s) => (
          <Link
            key={s.href}
            href={s.href}
            className="group rounded-lg border border-gray-200 bg-white p-5 transition-all hover:-translate-y-0.5 hover:border-blue-300 hover:shadow-md"
          >
            <div className="mb-3 inline-flex h-10 w-10 items-center justify-center rounded-lg bg-blue-50 text-blue-600 transition-colors group-hover:bg-blue-100">
              {s.icon}
            </div>
            <h2 className="font-semibold text-gray-900">{s.title}</h2>
            <p className="mt-1 text-sm text-gray-500">{s.description}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
