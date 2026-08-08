"use client";

import { useState } from "react";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import { useAuth } from "@/contexts/AuthContext";
import { formatDateTime, toDatetimeLocalInput } from "@/lib/format";
import type { AssignmentDto } from "@/types/assignment";
import type { TeacherAssignmentDto } from "@/types/teacherAssignment";

const createSchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().min(1, "Description is required"),
  slotId: z.string().min(1, "Select a class & subject"),
  deadline: z.string().min(1, "Deadline is required"),
  maxMarks: z.number().int().positive("Max marks must be greater than 0"),
});
type CreateValues = z.infer<typeof createSchema>;

const editSchema = z.object({
  title: z.string().min(1, "Title is required"),
  description: z.string().min(1, "Description is required"),
  deadline: z.string().min(1, "Deadline is required"),
  maxMarks: z.number().int().positive("Max marks must be greater than 0"),
});
type EditValues = z.infer<typeof editSchema>;

export default function TeacherAssignmentsPage() {
  const { user } = useAuth();
  const {
    data: assignments,
    isLoading,
    error,
    refetch,
  } = useApiQuery<AssignmentDto[]>("/api/assignments");
  const { data: slots } = useApiQuery<TeacherAssignmentDto[]>(
    user ? `/api/teacher-assignments?teacherId=${user.userId}` : null
  );

  const [mode, setMode] = useState<"none" | "create" | "edit">("none");
  const [editingAssignment, setEditingAssignment] = useState<AssignmentDto | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const createForm = useForm<CreateValues>({ resolver: zodResolver(createSchema) });
  const editForm = useForm<EditValues>({ resolver: zodResolver(editSchema) });

  function openCreate() {
    createForm.reset({ title: "", description: "", slotId: "", deadline: "", maxMarks: 100 });
    setFormError(null);
    setMode("create");
  }

  function openEdit(a: AssignmentDto) {
    setEditingAssignment(a);
    editForm.reset({
      title: a.title,
      description: a.description,
      deadline: toDatetimeLocalInput(a.deadline),
      maxMarks: a.maxMarks,
    });
    setFormError(null);
    setMode("edit");
  }

  function closeForm() {
    setMode("none");
    setEditingAssignment(null);
    setFormError(null);
  }

  async function onCreateSubmit(values: CreateValues) {
    setFormError(null);
    const slot = slots?.find((s) => s.id === values.slotId);
    if (!slot) {
      setFormError("Invalid class & subject selection");
      return;
    }
    try {
      await api.post("/api/assignments", {
        title: values.title,
        description: values.description,
        classId: slot.classId,
        subjectId: slot.subjectId,
        deadline: new Date(values.deadline).toISOString(),
        maxMarks: values.maxMarks,
      });
      refetch();
      closeForm();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to create assignment");
    }
  }

  async function onEditSubmit(values: EditValues) {
    if (!editingAssignment) return;
    setFormError(null);
    try {
      await api.put(`/api/assignments/${editingAssignment.id}`, {
        id: editingAssignment.id,
        title: values.title,
        description: values.description,
        deadline: new Date(values.deadline).toISOString(),
        maxMarks: values.maxMarks,
      });
      refetch();
      closeForm();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to update assignment");
    }
  }

  async function onDelete(id: string) {
    if (!confirm("Delete this assignment? This will fail if students have already submitted to it."))
      return;
    try {
      await api.delete(`/api/assignments/${id}`);
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to delete assignment");
    }
  }

  async function onToggleStatus(a: AssignmentDto) {
    const newStatus = a.status === "Published" ? "Draft" : "Published";
    try {
      await api.patch(`/api/assignments/${a.id}/status`, { id: a.id, status: newStatus });
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to update status");
    }
  }

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-gray-900">My Assignments</h1>
        <button
          onClick={openCreate}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          + New Assignment
        </button>
      </div>

      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="space-y-3">
        {assignments?.map((a) => (
          <div key={a.id} className="rounded-lg border border-gray-200 bg-white p-4">
            <div className="flex items-start justify-between">
              <div>
                <Link href={`/teacher/assignments/${a.id}`} className="font-semibold text-gray-900 hover:underline">
                  {a.title}
                </Link>
                <p className="text-sm text-gray-500">
                  {a.className} — {a.subjectName} · Deadline: {formatDateTime(a.deadline)} · Max marks:{" "}
                  {a.maxMarks}
                </p>
              </div>
              <span
                className={`rounded-full px-2 py-0.5 text-xs font-medium ${
                  a.status === "Published" ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"
                }`}
              >
                {a.status}
              </span>
            </div>
            <div className="mt-3 flex gap-4 text-sm">
              <button onClick={() => openEdit(a)} className="text-blue-600 hover:underline">
                Edit
              </button>
              <button onClick={() => onToggleStatus(a)} className="text-blue-600 hover:underline">
                {a.status === "Published" ? "Unpublish" : "Publish"}
              </button>
              <button onClick={() => onDelete(a.id)} className="text-red-600 hover:underline">
                Delete
              </button>
              <Link href={`/teacher/assignments/${a.id}`} className="text-gray-600 hover:underline">
                View Submissions
              </Link>
            </div>
          </div>
        ))}
        {assignments?.length === 0 && <p className="text-sm text-gray-500">No assignments yet.</p>}
      </div>

      {mode !== "none" && (
        <div className="fixed inset-0 flex items-center justify-center bg-black/30 px-4">
          <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
            <h2 className="mb-4 text-lg font-semibold text-gray-900">
              {mode === "create" ? "New Assignment" : "Edit Assignment"}
            </h2>

            {mode === "create" ? (
              <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Title</label>
                  <input
                    {...createForm.register("title")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.title && (
                    <p className="mt-1 text-sm text-red-600">{createForm.formState.errors.title.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Description</label>
                  <textarea
                    {...createForm.register("description")}
                    rows={3}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.description && (
                    <p className="mt-1 text-sm text-red-600">
                      {createForm.formState.errors.description.message}
                    </p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Class & Subject</label>
                  <select
                    {...createForm.register("slotId")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  >
                    <option value="">Select...</option>
                    {slots?.map((s) => (
                      <option key={s.id} value={s.id}>
                        {s.className} — {s.subjectName}
                      </option>
                    ))}
                  </select>
                  {createForm.formState.errors.slotId && (
                    <p className="mt-1 text-sm text-red-600">{createForm.formState.errors.slotId.message}</p>
                  )}
                  {slots?.length === 0 && (
                    <p className="mt-1 text-sm text-amber-600">
                      You haven&apos;t been assigned to teach any subjects yet — ask an Admin to assign you one.
                    </p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Deadline</label>
                  <input
                    type="datetime-local"
                    {...createForm.register("deadline")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.deadline && (
                    <p className="mt-1 text-sm text-red-600">{createForm.formState.errors.deadline.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Max Marks</label>
                  <input
                    type="number"
                    {...createForm.register("maxMarks", { valueAsNumber: true })}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.maxMarks && (
                    <p className="mt-1 text-sm text-red-600">{createForm.formState.errors.maxMarks.message}</p>
                  )}
                </div>
                {formError && <p className="text-sm text-red-600">{formError}</p>}
                <div className="flex justify-end gap-2 pt-2">
                  <button
                    type="button"
                    onClick={closeForm}
                    className="rounded-md px-4 py-2 text-sm text-gray-600 hover:bg-gray-100"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={createForm.formState.isSubmitting}
                    className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
                  >
                    Create
                  </button>
                </div>
              </form>
            ) : (
              <form onSubmit={editForm.handleSubmit(onEditSubmit)} className="space-y-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Title</label>
                  <input
                    {...editForm.register("title")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.title && (
                    <p className="mt-1 text-sm text-red-600">{editForm.formState.errors.title.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Description</label>
                  <textarea
                    {...editForm.register("description")}
                    rows={3}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.description && (
                    <p className="mt-1 text-sm text-red-600">
                      {editForm.formState.errors.description.message}
                    </p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Deadline</label>
                  <input
                    type="datetime-local"
                    {...editForm.register("deadline")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.deadline && (
                    <p className="mt-1 text-sm text-red-600">{editForm.formState.errors.deadline.message}</p>
                  )}
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700">Max Marks</label>
                  <input
                    type="number"
                    {...editForm.register("maxMarks", { valueAsNumber: true })}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.maxMarks && (
                    <p className="mt-1 text-sm text-red-600">{editForm.formState.errors.maxMarks.message}</p>
                  )}
                </div>
                {formError && <p className="text-sm text-red-600">{formError}</p>}
                <div className="flex justify-end gap-2 pt-2">
                  <button
                    type="button"
                    onClick={closeForm}
                    className="rounded-md px-4 py-2 text-sm text-gray-600 hover:bg-gray-100"
                  >
                    Cancel
                  </button>
                  <button
                    type="submit"
                    disabled={editForm.formState.isSubmitting}
                    className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
                  >
                    Save
                  </button>
                </div>
              </form>
            )}
          </div>
        </div>
      )}
    </div>
  );
}
