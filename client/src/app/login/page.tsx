"use client";
import { useAuth } from "@/contexts/AuthContext";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useState } from "react";
import { useForm } from "react-hook-form";
import {z} from "zod"
import Image from "next/image";

const loginSchema = z.object({
    email: z.string().email("Enter a valid email address"),
    password: z.string().min(1, "Pass is required")
});

type LoginFormValues = z.infer<typeof loginSchema>;

const roleHomePage : Record<string, string> = {
    Admin: "/admin",
    Teacher: "/teacher",
    Student: "/student"
};

export default function LoginPage() {
    const {login} = useAuth();
    const router = useRouter();
    const [serverError, setServerError] = useState<string | null>(null);

    const {
        register,
        handleSubmit,
        formState: {errors, isSubmitting}
    } = useForm<LoginFormValues>({
        resolver: zodResolver(loginSchema)
    });


    async function onSubmit(values: LoginFormValues) {
        setServerError(null);
        try{
            const user = await login(values.email, values.password);
            router.push(roleHomePage[user.role] ?? "/");
        } catch(err) {
            setServerError(err instanceof Error ? err.message : "Login failed");
        }
    }

    return (
        <div className="relative flex min-h-screen items-center justify-center overflow-hidden bg-gray-900 px-4">
        <Image
            src="/classroom_image.avif"
            alt=""
            fill
            priority
            className="scale-110 object-cover blur-sm"
        />
        <div className="absolute inset-0 bg-linear-to-b from-blue-950/80 via-blue-950/70 to-gray-900/85" />

        <div className="relative w-full max-w-sm rounded-xl border border-white/10 bg-white/95 p-8 shadow-2xl backdrop-blur-sm">
            <h1 className="mb-6 text-center text-2xl font-semibold text-gray-900">
            Assignment & Submission System
            </h1>
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
            <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                Email
                </label>
                <input
                id="email"
                type="email"
                autoComplete="email"
                {...register("email")}
                className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                />
                {errors.email && <p className="mt-1 text-sm text-red-600">{errors.email.message}</p>}
            </div>

            <div>
                <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                Password
                </label>
                <input
                id="password"
                type="password"
                autoComplete="current-password"
                {...register("password")}
                className="mt-1 w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:border-blue-500 focus:outline-none"
                />
                {errors.password && (
                <p className="mt-1 text-sm text-red-600">{errors.password.message}</p>
                )}
            </div>

            {serverError && (
                <p className="rounded-md bg-red-50 px-3 py-2 text-sm text-red-700">{serverError}</p>
            )}

            <button
                type="submit"
                disabled={isSubmitting}
                className="w-full rounded-md bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700 disabled:opacity-50"
            >
                {isSubmitting ? "Signing in..." : "Sign in"}
            </button>
            </form>
        </div>
        </div>
    );
}