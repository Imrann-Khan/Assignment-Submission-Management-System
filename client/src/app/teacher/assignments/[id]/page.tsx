"use client";

import { use, useState } from "react";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import { formatDateTime } from "@/lib/format";
import { Pagination } from "@/components/Pagination";
import type { AssignmentDto } from "@/types/assignment";
import type { SubmissionDto, SubmissionStatus } from "@/types/submission";
import type { PagedResult } from "@/types/pagination";

const gradeSchema = z.object({
  marks: z.number().int().min(0, "Marks cannot be negative"),
  feedback: z.string().optional(),
});
type GradeValues = z.infer<typeof gradeSchema>;

export default function TeacherAssignmentDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);

  const { data: assignment, error: assignmentError } = useApiQuery<AssignmentDto>(`/api/assignments/${id}`);
  const [pageNumber, setPageNumber] = useState(1);
  const {
    data,
    isLoading,
    error,
    refetch,
  } = useApiQuery<PagedResult<SubmissionDto>>(
    `/api/submissions?assignmentId=${id}&pageNumber=${pageNumber}&pageSize=15`
  );
  const submissions = data?.items;

  const [gradingId, setGradingId] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const { register, handleSubmit, reset, formState } = useForm<GradeValues>({
    resolver: zodResolver(gradeSchema),
  });

  function openGrade(submission: SubmissionDto) {
    reset({ marks: submission.marks ?? 0, feedback: submission.feedback ?? "" });
    setFormError(null);
    setGradingId(submission.id);
  }

  async function onGradeSubmit(values: GradeValues) {
    if (!gradingId) return;
    setFormError(null);
    try {
      await api.patch(`/api/submissions/${gradingId}/grade`, {
        id: gradingId,
        marks: values.marks,
        feedback: values.feedback || null,
      });
      refetch();
      setGradingId(null);
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to grade submission");
    }
  }

  async function onStatusChange(submissionId: string, status: SubmissionStatus) {
    try {
      await api.patch(`/api/submissions/${submissionId}/status`, { id: submissionId, status });
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to update status");
    }
  }

  return (
    <div>
      <Link href="/teacher" className="text-sm text-blue-600 hover:underline">
        &larr; Back to My Assignments
      </Link>

      {assignmentError && <p className="mt-4 text-sm text-red-600">{assignmentError}</p>}

      {assignment && (
        <div className="mt-4 mb-6 rounded-lg border border-gray-200 bg-white p-4">
          <h1 className="text-xl font-semibold text-gray-900">{assignment.title}</h1>
          <p className="mt-1 text-sm text-gray-600">{assignment.description}</p>
          <p className="mt-2 text-sm text-gray-500">
            {assignment.className} — {assignment.subjectName} · Deadline: {formatDateTime(assignment.deadline)} ·
            Max marks: {assignment.maxMarks} · Status: {assignment.status}
          </p>
        </div>
      )}

      <h2 className="mb-3 text-lg font-semibold text-gray-900">Submissions</h2>
      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="space-y-3">
        {submissions?.map((s) => (
          <div key={s.id} className="rounded-lg border border-gray-200 bg-white p-4">
            <div className="flex items-start justify-between">
              <div>
                <p className="font-medium text-gray-900">{s.studentName}</p>
                <p className="text-sm text-gray-500">Submitted: {formatDateTime(s.submittedAt)}</p>
              </div>
              <select
                value={s.status}
                onChange={(e) => onStatusChange(s.id, e.target.value as SubmissionStatus)}
                className={`rounded-full border-0 px-2 py-0.5 text-xs font-medium ${
                  s.status === "Graded"
                    ? "bg-green-100 text-green-700"
                    : s.status === "Returned"
                      ? "bg-yellow-100 text-yellow-700"
                      : "bg-blue-100 text-blue-700"
                }`}
              >
                <option value="Submitted">Submitted</option>
                <option value="Graded">Graded</option>
                <option value="Returned">Returned</option>
              </select>
            </div>
            <p className="mt-2 whitespace-pre-wrap text-sm text-gray-700">{s.answerText}</p>

            {s.status === "Graded" && (
              <p className="mt-2 text-sm text-gray-600">
                Marks: {s.marks} / {s.maxMarks}
                {s.feedback && <> — {s.feedback}</>}
              </p>
            )}

            {gradingId === s.id ? (
              <form onSubmit={handleSubmit(onGradeSubmit)} className="mt-3 space-y-2 rounded-md bg-gray-50 p-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Marks (out of {s.maxMarks})</label>
                  <input
                    type="number"
                    {...register("marks", { valueAsNumber: true })}
                    className="mt-1 w-32 rounded-md border border-gray-300 px-3 py-1.5 text-sm"
                  />
                  {formState.errors.marks && (
                    <p className="mt-1 text-sm text-red-600">{formState.errors.marks.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Feedback</label>
                  <textarea
                    {...register("feedback")}
                    rows={2}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-1.5 text-sm"
                  />
                </div>
                {formError && <p className="text-sm text-red-600">{formError}</p>}
                <div className="flex gap-2">
                  <button
                    type="submit"
                    disabled={formState.isSubmitting}
                    className="rounded-md bg-blue-600 px-3 py-1.5 text-sm text-white hover:bg-blue-700 disabled:opacity-50"
                  >
                    Save Grade
                  </button>
                  <button
                    type="button"
                    onClick={() => setGradingId(null)}
                    className="rounded-md px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-100"
                  >
                    Cancel
                  </button>
                </div>
              </form>
            ) : (
              <button onClick={() => openGrade(s)} className="mt-3 text-sm text-blue-600 hover:underline">
                {s.status === "Graded" ? "Update Grade" : "Grade Submission"}
              </button>
            )}
          </div>
        ))}
        {submissions?.length === 0 && <p className="text-sm text-gray-500">No submissions yet.</p>}
      </div>

      <Pagination
        currentPage={data?.pageNumber ?? 1}
        totalPages={data?.totalPages ?? 1}
        onPageChange={setPageNumber}
      />
    </div>
  );
}
