"use client";

import { useState } from "react";
import { useApiQuery } from "@/lib/useApiQuery";
import { Pagination } from "@/components/Pagination";
import { LoadingState } from "@/components/LoadingState";
import { ErrorAlert } from "@/components/ErrorAlert";
import { formatDateTime } from "@/lib/format";
import type { SubmissionDto } from "@/types/submission";
import type { PagedResult } from "@/types/pagination";

export default function AdminSubmissionsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { data, isLoading, error } = useApiQuery<PagedResult<SubmissionDto>>(
    `/api/submissions?pageNumber=${pageNumber}&pageSize=10`
  );
  const submissions = data?.items;

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">All Submissions</h1>

      {isLoading && <LoadingState />}
      {error && <ErrorAlert message={error} />}

      {submissions && (
        <>
          <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200 text-sm">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Assignment</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Student</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Submitted</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Status</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Marks</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Graded By</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {submissions.map((s) => (
                  <tr key={s.id} className="transition-colors hover:bg-gray-50">
                    <td className="px-4 py-2 font-medium text-gray-900">{s.assignmentTitle}</td>
                    <td className="px-4 py-2 text-gray-600">{s.studentName}</td>
                    <td className="px-4 py-2 text-gray-600">{formatDateTime(s.submittedAt)}</td>
                    <td className="px-4 py-2">
                      <span
                        className={`rounded-full px-2 py-0.5 text-xs font-medium ${
                          s.status === "Graded"
                            ? "bg-green-100 text-green-700"
                            : s.status === "Returned"
                              ? "bg-yellow-100 text-yellow-700"
                              : "bg-blue-100 text-blue-700"
                        }`}
                      >
                        {s.status}
                      </span>
                    </td>
                    <td className="px-4 py-2 text-gray-600">
                      {s.marks !== null ? `${s.marks} / ${s.maxMarks}` : "-"}
                    </td>
                    <td className="px-4 py-2 text-gray-600">{s.gradedByName ?? "-"}</td>
                  </tr>
                ))}
                {submissions.length === 0 && (
                  <tr>
                    <td colSpan={6} className="px-4 py-6 text-center text-sm text-gray-500">
                      No submissions yet.
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
