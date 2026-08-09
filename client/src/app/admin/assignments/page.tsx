"use client";

import { useState } from "react";
import { useApiQuery } from "@/lib/useApiQuery";
import { Pagination } from "@/components/Pagination";
import { formatDateTime } from "@/lib/format";
import type { AssignmentDto } from "@/types/assignment";
import type { PagedResult } from "@/types/pagination";

export default function AdminAssignmentsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { data, isLoading, error } = useApiQuery<PagedResult<AssignmentDto>>(
    `/api/assignments?pageNumber=${pageNumber}&pageSize=10`
  );
  const assignments = data?.items;

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">All Assignments</h1>

      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      {assignments && (
        <>
          <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200 text-sm">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Title</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Class</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Subject</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Teacher</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Deadline</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Status</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {assignments.map((a) => (
                  <tr key={a.id}>
                    <td className="px-4 py-2 font-medium text-gray-900">{a.title}</td>
                    <td className="px-4 py-2 text-gray-600">{a.className}</td>
                    <td className="px-4 py-2 text-gray-600">{a.subjectName}</td>
                    <td className="px-4 py-2 text-gray-600">{a.teacherName}</td>
                    <td className="px-4 py-2 text-gray-600">{formatDateTime(a.deadline)}</td>
                    <td className="px-4 py-2">
                      <span
                        className={`rounded-full px-2 py-0.5 text-xs font-medium ${
                          a.status === "Published" ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"
                        }`}
                      >
                        {a.status}
                      </span>
                    </td>
                  </tr>
                ))}
                {assignments.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                      No assignments yet.
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
          <Pagination
            currentPage={data?.pageNumber ?? 1}
            totalPages={data?.totalPages ?? 1}
            onPageChange={setPageNumber}
          />
        </>
      )}
    </div>
  );
}
