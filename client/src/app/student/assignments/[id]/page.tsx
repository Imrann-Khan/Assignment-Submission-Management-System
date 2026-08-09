"use client";

import { use, useEffect, useState } from "react";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import { formatDateTime } from "@/lib/format";
import type { AssignmentDto } from "@/types/assignment";
import type { SubmissionDto } from "@/types/submission";
import type { PagedResult } from "@/types/pagination";

const submitSchema = z.object({
  answerText: z.string().min(1, "Answer cannot be empty"),
});
type SubmitValues = z.infer<typeof submitSchema>;

export default function StudentAssignmentDetailPage({
  params,
}: {
  params: Promise<{ id: string }>;
}) {
  const { id } = use(params);

  const { data: assignment, error: assignmentError } = useApiQuery<AssignmentDto>(`/api/assignments/${id}`);
  const {
    data: submissionsResult,
    isLoading,
    error,
    refetch,
  } = useApiQuery<PagedResult<SubmissionDto>>(`/api/submissions?assignmentId=${id}`);
  const submissions = submissionsResult?.items;

  const mySubmission = submissions?.[0] ?? null;
  const [formError, setFormError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const { register, handleSubmit, reset, formState } = useForm<SubmitValues>({
    resolver: zodResolver(submitSchema),
  });

  useEffect(() => {
    if (mySubmission) {
      reset({ answerText: mySubmission.answerText });
    }
  }, [mySubmission, reset]);

  const isPastDeadline = assignment ? new Date(assignment.deadline).getTime() < Date.now() : false;

  async function onSubmit(values: SubmitValues) {
    setFormError(null);
    setSuccessMessage(null);
    try {
      await api.post("/api/submissions/submit", { assignmentId: id, answerText: values.answerText });
      setSuccessMessage(mySubmission ? "Submission updated." : "Submitted successfully.");
      refetch();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to submit");
    }
  }

  return (
    <div>
      <Link href="/student" className="text-sm text-blue-600 hover:underline">
        &larr; Back to My Assignments
      </Link>

      {assignmentError && <p className="mt-4 text-sm text-red-600">{assignmentError}</p>}

      {assignment && (
        <div className="mt-4 mb-6 rounded-lg border border-gray-200 bg-white p-4">
          <h1 className="text-xl font-semibold text-gray-900">{assignment.title}</h1>
          <p className="mt-1 whitespace-pre-wrap text-sm text-gray-600">{assignment.description}</p>
          <p className="mt-2 text-sm text-gray-500">
            {assignment.className} — {assignment.subjectName} · Deadline: {formatDateTime(assignment.deadline)} ·
            Max marks: {assignment.maxMarks}
          </p>
        </div>
      )}

      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      {mySubmission?.status === "Graded" && (
        <div className="mb-6 rounded-lg border border-green-200 bg-green-50 p-4">
          <p className="font-medium text-green-800">
            Graded: {mySubmission.marks} / {mySubmission.maxMarks}
          </p>
          {mySubmission.feedback && <p className="mt-1 text-sm text-green-700">{mySubmission.feedback}</p>}
        </div>
      )}

      {mySubmission?.status === "Returned" && (
        <div className="mb-6 rounded-lg border border-yellow-200 bg-yellow-50 p-4">
          <p className="font-medium text-yellow-800">This submission was returned by your teacher.</p>
          {mySubmission.feedback && <p className="mt-1 text-sm text-yellow-700">{mySubmission.feedback}</p>}
        </div>
      )}

      <div className="rounded-lg border border-gray-200 bg-white p-4">
        <h2 className="mb-3 font-semibold text-gray-900">{mySubmission ? "Your Answer" : "Submit Your Answer"}</h2>

        {isPastDeadline && !mySubmission && (
          <p className="text-sm text-red-600">The deadline for this assignment has passed.</p>
        )}

        {isPastDeadline && mySubmission && (
          <p className="mb-3 text-sm text-gray-500">
            The deadline has passed — your submission can no longer be changed.
          </p>
        )}

        {!isPastDeadline && (
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-3">
            <textarea
              {...register("answerText")}
              rows={6}
              className="w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
              placeholder="Write your answer here..."
            />
            {formState.errors.answerText && (
              <p className="text-sm text-red-600">{formState.errors.answerText.message}</p>
            )}
            {formError && <p className="text-sm text-red-600">{formError}</p>}
            {successMessage && <p className="text-sm text-green-600">{successMessage}</p>}
            <button
              type="submit"
              disabled={formState.isSubmitting}
              className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
            >
              {mySubmission ? "Update Submission" : "Submit"}
            </button>
          </form>
        )}

        {isPastDeadline && mySubmission && (
          <p className="whitespace-pre-wrap rounded-md bg-gray-50 p-3 text-sm text-gray-700">
            {mySubmission.answerText}
          </p>
        )}
      </div>
    </div>
  );
}
