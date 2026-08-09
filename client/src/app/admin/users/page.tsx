"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { api, ApiError } from "@/lib/api";
import { useApiQuery } from "@/lib/useApiQuery";
import type { UserDto } from "@/types/user";
import type { ClassDto } from "@/types/class";
import type { PagedResult } from "@/types/pagination";
import { Pagination } from "@/components/Pagination";

const roles = ["Admin", "Teacher", "Student"] as const;

const createUserSchema = z
  .object({
    fullName: z.string().min(1, "Full name is required"),
    email: z.string().email("Enter a valid email"),
    password: z.string().min(6, "Password must be at least 6 characters"),
    role: z.enum(roles),
    classId: z.string().optional(),
  })
  .refine((data) => data.role !== "Student" || !!data.classId, {
    message: "Class is required for students",
    path: ["classId"],
  });

type CreateUserValues = z.infer<typeof createUserSchema>;

const editUserSchema = z.object({
  fullName: z.string().min(1, "Full name is required"),
  email: z.string().email("Enter a valid email"),
  classId: z.string().optional(),
});

type EditUserValues = z.infer<typeof editUserSchema>;

export default function UsersPage() {
  const [pageNumber, setPageNumber] = useState(1);
  const { data, isLoading, error, refetch } = useApiQuery<PagedResult<UserDto>>(`/api/users?pageNumber=${pageNumber}&pageSize=10`); // Default page size = 10
  const users = data?.items;
  const { data: classesResult } = useApiQuery<PagedResult<ClassDto>>("/api/classes?pageSize=100");
  const classes = classesResult?.items;

  const [mode, setMode] = useState<"none" | "create" | "edit">("none");
  const [editingUser, setEditingUser] = useState<UserDto | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const createForm = useForm<CreateUserValues>({
    resolver: zodResolver(createUserSchema),
    defaultValues: { role: "Student" },
  });

  const editForm = useForm<EditUserValues>({
    resolver: zodResolver(editUserSchema),
  });

  const watchedRole = createForm.watch("role");

  function openCreate() {
    createForm.reset({ fullName: "", email: "", password: "", role: "Student", classId: "" });
    setFormError(null);
    setMode("create");
  }

  function openEdit(user: UserDto) {
    setEditingUser(user);
    editForm.reset({
      fullName: user.fullName,
      email: user.email,
      classId: user.classId ?? "",
    });
    setFormError(null);
    setMode("edit");
  }

  function closeForm() {
    setMode("none");
    setEditingUser(null);
    setFormError(null);
  }

  async function onCreateSubmit(values: CreateUserValues) {
    setFormError(null);
    try {
      await api.post("/api/users", {
        fullName: values.fullName,
        email: values.email,
        password: values.password,
        role: values.role,
        classId: values.role === "Student" ? values.classId : null,
      });
      refetch();
      closeForm();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to create user");
    }
  }

  async function onEditSubmit(values: EditUserValues) {
    if (!editingUser) return;
    setFormError(null);
    try {
      await api.put(`/api/users/${editingUser.id}`, {
        id: editingUser.id,
        fullName: values.fullName,
        email: values.email,
        classId: editingUser.role === "Student" ? values.classId : null,
      });
      refetch();
      closeForm();
    } catch (err) {
      setFormError(err instanceof ApiError ? err.message : "Failed to update user");
    }
  }

  async function toggleActive(user: UserDto) {
    try {
      await api.patch(`/api/users/${user.id}/status`, {
        id: user.id,
        isActive: !user.isActive,
      });
      refetch();
    } catch (err) {
      alert(err instanceof ApiError ? err.message : "Failed to update status");
    }
  }

  return (
    <div>
      <div className="mb-6 flex items-center justify-between">
        <h1 className="text-xl font-semibold text-gray-900">Users</h1>
        <button
          onClick={openCreate}
          className="rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
        >
          + New User
        </button>
      </div>

      {isLoading && <p className="text-sm text-gray-500">Loading...</p>}
      {error && <p className="text-sm text-red-600">{error}</p>}

      {users && (
        <>
          <div className="overflow-x-auto rounded-lg border border-gray-200 bg-white">
            <table className="min-w-full divide-y divide-gray-200 text-sm">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Name</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Email</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Role</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Class</th>
                <th className="px-4 py-2 text-left font-medium text-gray-600">Status</th>
                <th className="px-4 py-2 text-right font-medium text-gray-600">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-100">
              {users.map((user) => (
                <tr key={user.id}>
                  <td className="px-4 py-2">{user.fullName}</td>
                  <td className="px-4 py-2 text-gray-600">{user.email}</td>
                  <td className="px-4 py-2">{user.role}</td>
                  <td className="px-4 py-2 text-gray-600">{user.className ?? "—"}</td>
                  <td className="px-4 py-2">
                    <span
                      className={`rounded-full px-2 py-0.5 text-xs font-medium ${
                        user.isActive ? "bg-green-100 text-green-700" : "bg-gray-100 text-gray-600"
                      }`}
                    >
                      {user.isActive ? "Active" : "Inactive"}
                    </span>
                  </td>
                  <td className="space-x-3 px-4 py-2 text-right">
                    <button onClick={() => openEdit(user)} className="text-blue-600 hover:underline">
                      Edit
                    </button>
                    <button onClick={() => toggleActive(user)} className="text-gray-600 hover:underline">
                      {user.isActive ? "Deactivate" : "Activate"}
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

      {mode !== "none" && (
        <div className="fixed inset-0 flex items-center justify-center bg-black/30 px-4">
          <div className="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
            <h2 className="mb-4 text-lg font-semibold text-gray-900">
              {mode === "create" ? "Create User" : "Edit User"}
            </h2>

            {mode === "create" ? (
              <form onSubmit={createForm.handleSubmit(onCreateSubmit)} className="space-y-3">
                <div>
                  <label className="block text-sm font-medium text-gray-700">Full Name</label>
                  <input
                    {...createForm.register("fullName")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.fullName && (
                    <p className="mt-1 text-sm text-red-600">
                      {createForm.formState.errors.fullName.message}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Email</label>
                  <input
                    {...createForm.register("email")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.email && (
                    <p className="mt-1 text-sm text-red-600">
                      {createForm.formState.errors.email.message}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Password</label>
                  <input
                    type="password"
                    {...createForm.register("password")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {createForm.formState.errors.password && (
                    <p className="mt-1 text-sm text-red-600">
                      {createForm.formState.errors.password.message}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Role</label>
                  <select
                    {...createForm.register("role")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  >
                    {roles.map((role) => (
                      <option key={role} value={role}>
                        {role}
                      </option>
                    ))}
                  </select>
                </div>

                {watchedRole === "Student" && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Class</label>
                    <select
                      {...createForm.register("classId")}
                      className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                    >
                      <option value="">Select a class</option>
                      {classes?.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name}
                        </option>
                      ))}
                    </select>
                    {createForm.formState.errors.classId && (
                      <p className="mt-1 text-sm text-red-600">
                        {createForm.formState.errors.classId.message}
                      </p>
                    )}
                  </div>
                )}

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
                  <label className="block text-sm font-medium text-gray-700">Full Name</label>
                  <input
                    {...editForm.register("fullName")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.fullName && (
                    <p className="mt-1 text-sm text-red-600">
                      {editForm.formState.errors.fullName.message}
                    </p>
                  )}
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700">Email</label>
                  <input
                    {...editForm.register("email")}
                    className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                  />
                  {editForm.formState.errors.email && (
                    <p className="mt-1 text-sm text-red-600">
                      {editForm.formState.errors.email.message}
                    </p>
                  )}
                </div>

                {editingUser?.role === "Student" && (
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Class</label>
                    <select
                      {...editForm.register("classId")}
                      className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm"
                    >
                      <option value="">Select a class</option>
                      {classes?.map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name}
                        </option>
                      ))}
                    </select>
                  </div>
                )}

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
