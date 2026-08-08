"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import type { ClassDto } from "@/types/class";

const classSchema = z.object({ name: z.string().min(1, "Class name is required") });
type ClassValues = z.infer<typeof classSchema>;

const subjectSchema = z.object({ name: z.string().min(1, "Subject name is required") });
type SubjectValues = z.infer<typeof subjectSchema>;

export default function ClassesPage() {
  const { data: classes, isLoading, error, refetch } = useApiQuery<ClassDto[]>("/api/classes");
  const [formError, setFormError] = useState<string | null>(null);
  const [addingSubjectFor, setAddingSubjectFor] = useState<string | null>(null);

  const classForm = useForm<ClassValues>({ resolver: zodResolver(classSchema) });
  const subjectForm = useForm<SubjectValues>({ resolver: zodResolver(subjectSchema) });

  async function onCreateClass(values: ClassValues) {
    setFormError(null);
    try {
      await api.post("/api/classes", values);
      classForm.reset({ name: "" });
      refetch();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to create class");
    }
  }

  async function onDeleteClass(id: string) {
    if (
      !confirm(
        "Delete this class? This will fail if it still has students, subjects, or assignments."
      )
    )
      return;
    try {
      await api.delete(`/api/classes/${id}`);
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to delete class");
    }
  }

  async function onCreateSubject(values: SubjectValues, classId: string) {
    setFormError(null);
    try {
      await api.post("/api/subjects", { name: values.name, classId });
      subjectForm.reset({ name: "" });
      setAddingSubjectFor(null);
      refetch();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to create subject");
    }
  }

  async function onDeleteSubject(id: string) {
    if (
      !confirm("Delete this subject? This will fail if it still has assignments or teacher assignments.")
    )
      return;
    try {
      await api.delete(`/api/subjects/${id}`);
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to delete subject");
    }
  }

  return (
    <div>
      <h1 className="mb-6 text-xl font-semibold text-gray-900">Classes & Subjects</h1>

      <form
        onSubmit={classForm.handleSubmit(onCreateClass)}
        className="mb-8 flex items-end gap-3 rounded-lg border border-gray-200 bg-white p-4"
      >
        <div className="flex-1">
          <label className="block text-sm font-medium text-gray-700">New Class Name</label>
          <input
            {...classForm.register("name")}
            placeholder="e.g. Class 10 - A"
            className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
          />
          {classForm.formState.errors.name && (
            <p className="mt-1 text-sm text-red-600">{classForm.formState.errors.name.message}</p>
          )}
        </div>
        <button
          type="submit"
          disabled={classForm.formState.isSubmitting}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
        >
          Add Class
        </button>
      </form>

      {formError && <p className="mb-4 text-sm text-red-600">{formError}</p>}
      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      <div className="space-y-4">
        {classes?.map((c) => (
          <div key={c.id} className="rounded-lg border border-gray-200 bg-white p-4">
            <div className="mb-3 flex items-center justify-between">
              <div>
                <h2 className="font-semibold text-gray-900">{c.name}</h2>
                <p className="text-sm text-gray-500">{c.studentCount} student(s)</p>
              </div>
              <button onClick={() => onDeleteClass(c.id)} className="text-sm text-red-600 hover:underline">
                Delete Class
              </button>
            </div>

            <ul className="mb-3 space-y-1">
              {c.subjects.map((s) => (
                <li
                  key={s.id}
                  className="flex items-center justify-between rounded-md bg-gray-50 px-3 py-1.5 text-sm"
                >
                  <span>{s.name}</span>
                  <button onClick={() => onDeleteSubject(s.id)} className="text-red-600 hover:underline">
                    Remove
                  </button>
                </li>
              ))}
              {c.subjects.length === 0 && <li className="text-sm text-gray-400">No subjects yet.</li>}
            </ul>

            {addingSubjectFor === c.id ? (
              <form
                onSubmit={subjectForm.handleSubmit((values) => onCreateSubject(values, c.id))}
                className="flex items-end gap-2"
              >
                <input
                  {...subjectForm.register("name")}
                  placeholder="Subject name"
                  className="rounded-md border border-gray-300 px-3 py-1.5 text-sm"
                  autoFocus
                />
                <button
                  type="submit"
                  className="rounded-md bg-blue-600 px-3 py-1.5 text-sm text-white hover:bg-blue-700"
                >
                  Add
                </button>
                <button
                  type="button"
                  onClick={() => setAddingSubjectFor(null)}
                  className="rounded-md px-3 py-1.5 text-sm text-gray-600 hover:bg-gray-100"
                >
                  Cancel
                </button>
              </form>
            ) : (
              <button
                onClick={() => {
                  subjectForm.reset({ name: "" });
                  setAddingSubjectFor(c.id);
                }}
                className="text-sm text-blue-600 hover:underline"
              >
                + Add Subject
              </button>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
