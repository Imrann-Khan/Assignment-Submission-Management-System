import { NextRequest, NextResponse } from "next/server";

const roleHomePage : Record<string, string> = {
    Admin: "/admin",
    Teacher: "/teacher",
    Student: "student"
};

export function proxy(request: NextRequest) {
    const {pathname} = request.nextUrl;
    const role = request.cookies.get("role")?.value;

    const isProtectedRoute = pathname.startsWith("/admin") || 
                                pathname.startsWith("/teacher") || pathname.startsWith("/student");

    if(isProtectedRoute && !role){
        return NextResponse.redirect(new URL("/login", request.url));
    }

    if(pathname.startsWith("/admin") && role !== "Admin"){
        return NextResponse.redirect(new URL("/login", request.url));
    }
    if (pathname.startsWith("/teacher") && role !== "Teacher") {
        return NextResponse.redirect(new URL(roleHomePage[role ?? ""] ?? "/login", request.url));
    }

    if (pathname.startsWith("/student") && role !== "Student") {
        return NextResponse.redirect(new URL(roleHomePage[role ?? ""] ?? "/login", request.url));
    }

    if(pathname === "/login" && role) {
        return NextResponse.redirect(new URL(roleHomePage[role ?? ""] ?? "/login", request.url));
    }

    return NextResponse.next();
}

export const config = {
  matcher: ["/admin/:path*", "/teacher/:path*", "/student/:path*", "/login"],
};