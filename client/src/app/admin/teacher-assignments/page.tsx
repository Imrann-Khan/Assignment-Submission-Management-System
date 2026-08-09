"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import { Pagination } from "@/components/Pagination";
import type { TeacherAssignmentDto } from "@/types/teacherAssignment";
import type { UserDto } from "@/types/user";
import type { ClassDto } from "@/types/class";
import type { PagedResult } from "@/types/pagination";

const assignmentSchema = z.object({
  teacherId: z.string().min(1, "Select a teacher"),
  subjectId: z.string().min(1, "Select a subject"),
});
type AssignmentValues = z.infer<typeof assignmentSchema>;

export default function TeacherAssignmentsPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const {
    data,
    isLoading,
    error,
    refetch,
  } = useApiQuery<PagedResult<TeacherAssignmentDto>>(
    `/api/teacher-assignments?pageNumber=${pageNumber}&pageSize=10`
  );
  const assignments = data?.items;
  const { data: teachersResult } = useApiQuery<PagedResult<UserDto>>("/api/users?role=Teacher&pageSize=100");
  const teachers = teachersResult?.items;
  const { data: classesResult } = useApiQuery<PagedResult<ClassDto>>("/api/classes?pageSize=100");
  const classes = classesResult?.items;

  const [formError, setFormError] = useState<string | null>(null);

  const { register, handleSubmit, reset, formState } = useForm<AssignmentValues>({
    resolver: zodResolver(assignmentSchema),
  });

  const subjectOptions = (classes ?? []).flatMap((c) =>
    c.subjects.map((s) => ({ id: s.id, label: `${c.name} — ${s.name}` }))
  );

  async function onSubmit(values: AssignmentValues) {
    setFormError(null);
    try {
      await api.post("/api/teacher-assignments", values);
      reset({ teacherId: "", subjectId: "" });
      refetch();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to create assignment");
    }
  }

  async function onDelete(id: string) {
    if (!confirm("Remove this teacher-subject assignment?")) return;
    try {
      await api.delete(`/api/teacher-assignments/${id}`);
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to remove assignment");
    }
  }

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">Teacher Assignments</h1>

      <form
        onSubmit={handleSubmit(onSubmit)}
        className="mb-8 flex flex-wrap items-end gap-3 rounded-lg border border-gray-200 bg-white p-4"
      >
        <div>
          <label className="block text-sm font-medium text-gray-700">Teacher</label>
          <select
            {...register("teacherId")}
            className="mt-1 rounded-md border border-gray-300 px-3 py-2 text-sm"
          >
            <option value="">Select a teacher</option>
            {teachers?.map((t) => (
              <option key={t.id} value={t.id}>
                {t.fullName}
              </option>
            ))}
          </select>
          {formState.errors.teacherId && (
            <p className="mt-1 text-sm text-red-600">{formState.errors.teacherId.message}</p>
          )}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Subject</label>
          <select
            {...register("subjectId")}
            className="mt-1 rounded-md border border-gray-300 px-3 py-2 text-sm"
          >
            <option value="">Select a subject</option>
            {subjectOptions.map((s) => (
              <option key={s.id} value={s.id}>
                {s.label}
              </option>
            ))}
          </select>
          {formState.errors.subjectId && (
            <p className="mt-1 text-sm text-red-600">{formState.errors.subjectId.message}</p>
          )}
        </div>

        <button
          type="submit"
          disabled={formState.isSubmitting}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
        >
          Assign
        </button>
      </form>

      {formError && <p className="mb-4 text-sm text-red-600">{formError}</p>}
      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      {assignments && (
        <>
          <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200 text-sm">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Teacher</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Subject</th>
                  <th className="px-4 py-2 text-left font-medium text-gray-600">Class</th>
                  <th className="px-4 py-2 text-right font-medium text-gray-600">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {assignments.map((a) => (
                  <tr key={a.id}>
                    <td className="px-4 py-2">{a.teacherName}</td>
                    <td className="px-4 py-2">{a.subjectName}</td>
                    <td className="px-4 py-2 text-gray-600">{a.className}</td>
                    <td className="px-4 py-2 text-right">
                      <button onClick={() => onDelete(a.id)} className="text-red-600 hover:underline">
                        Remove
                      </button>
                    </td>
                  </tr>
                ))}
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
