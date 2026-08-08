import path from "path";
import { optional } from "zod";

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5064";


export class ApiError extends Error {
    status: number;
    errors?: Record<string, string[]>;

    constructor(status: number, message: string, errors?: Record<string, string[]>) {
        super(message);
        this.status = status;
        this.errors = errors;
    }
}

function getToken(): string | null {
    if(typeof window === "undefined") return null;
    return localStorage.getItem("token");
}

async function request<TResponse>(path: string, options: RequestInit = {}): Promise<TResponse> {
    const token = getToken();

    const headers: HeadersInit = {
        "Content-Type": "application/json",
        ...(token ? {Authorization: `Bearer ${token}`}: {}),
        ...options.headers,
    };

    const response = await fetch(`${API_BASE_URL}${path}`, {
        ...options,
        headers,
    });

    if(response.status == 204) {
        return undefined as TResponse;
    }

    const data = await response.json().catch(()=>null);

    if(!response.ok) {
        const message = data?.detail ?? data?.title ?? "Something went wrong";
    }

    return data as TResponse;
}

export const api = {
  get: <T>(path: string) => request<T>(path, { method: "GET" }),
  post: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "POST", body: body ? JSON.stringify(body) : undefined }),
  put: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "PUT", body: body ? JSON.stringify(body) : undefined }),
  patch: <T>(path: string, body?: unknown) =>
    request<T>(path, { method: "PATCH", body: body ? JSON.stringify(body) : undefined }),
  delete: <T>(path: string) => request<T>(path, { method: "DELETE" }),
};
