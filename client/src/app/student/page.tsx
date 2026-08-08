"use client";

import Link from "next/link";
import { useApiQuery } from "@/lib/useApiQuery";
import { formatDateTime } from "@/lib/format";
import type { AssignmentDto } from "@/types/assignment";

export default function StudentAssignmentsPage() {
  const { data: assignments, isLoading, error } = useApiQuery<AssignmentDto[]>("/api/assignments");

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">My Assignments</h1>

      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="space-y-3">
        {assignments?.map((a) => {
          const isPastDeadline = new Date(a.deadline).getTime() < Date.now();
          return (
            <Link
              key={a.id}
              href={`/student/assignments/${a.id}`}
              className="block rounded-lg border border-gray-200 bg-white p-4 hover:border-blue-300 hover:shadow-sm"
            >
              <div className="flex items-start justify-between">
                <div>
                  <h2 className="font-semibold text-gray-900">{a.title}</h2>
                  <p className="text-sm text-gray-500">
                    {a.className} — {a.subjectName} · Max marks: {a.maxMarks}
                  </p>
                </div>
                <span className={`text-xs font-medium ${isPastDeadline ? "text-red-600" : "text-gray-500"}`}>
                  {isPastDeadline ? "Deadline passed" : `Due ${formatDateTime(a.deadline)}`}
                </span>
              </div>
            </Link>
          );
        })}
        {assignments?.length === 0 && <p className="text-sm text-gray-500">No assignments available yet.</p>}
      </div>
    </div>
  );
}
