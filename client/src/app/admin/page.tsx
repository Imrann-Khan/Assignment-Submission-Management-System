import Link from "next/link";

const sections = [
  {
    href: "/admin/users",
    title: "Users",
    description: "Create and manage Admin, Teacher, and Student accounts.",
  },
  {
    href: "/admin/classes",
    title: "Classes & Subjects",
    description: "Manage classes and the subjects taught within them.",
  },
  {
    href: "/admin/teacher-assignments",
    title: "Teacher Assignments",
    description: "Assign teachers to the subjects they teach.",
  },
];

export default function AdminHomePage() {
  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">Admin Dashboard</h1>
      <div className="grid gap-4 sm:grid-cols-3">
        {sections.map((s) => (
          <Link
            key={s.href}
            href={s.href}
            className="rounded-lg border border-gray-200 bg-white p-5 hover:border-blue-300 hover:shadow-sm"
          >
            <h2 className="font-semibold text-gray-900">{s.title}</h2>
            <p className="mt-1 text-sm text-gray-500">{s.description}</p>
          </Link>
        ))}
      </div>
    </div>
  );
}
