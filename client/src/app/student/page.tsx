"use client";

import { useState } from "react";
import Link from "next/link";
import { useApiQuery } from "@/lib/useApiQuery";
import { formatDateTime } from "@/lib/format";
import { Pagination } from "@/components/Pagination";
import { LoadingState } from "@/components/LoadingState";
import { ErrorAlert } from "@/components/ErrorAlert";
import { EmptyState } from "@/components/EmptyState";
import type { AssignmentDto } from "@/types/assignment";
import type { PagedResult } from "@/types/pagination";

export default function StudentAssignmentsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { data, isLoading, error } = useApiQuery<PagedResult<AssignmentDto>>(
    `/api/assignments?pageNumber=${pageNumber}&pageSize=10`
  );
  const assignments = data?.items;

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">My Assignments</h1>

      {isLoading && <LoadingState />}
      {error && <ErrorAlert message={error} />}

      {assignments?.length === 0 && <EmptyState message="No assignments available yet." />}

      <div className="space-y-3">
        {assignments?.map((a) => {
          const isPastDeadline = new Date(a.deadline).getTime() < Date.now();
          return (
            <Link
              key={a.id}
              href={`/student/assignments/${a.id}`}
              className="block rounded-lg border border-gray-200 bg-white p-4 transition-all hover:-translate-y-0.5 hover:border-blue-300 hover:shadow-md"
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
      </div>

      <Pagination
        currentPage={data?.pageNumber ?? 1}
        totalPages={data?.totalPages ?? 1}
        onPageChange={setPageNumber}
      />
    </div>
  );
}
