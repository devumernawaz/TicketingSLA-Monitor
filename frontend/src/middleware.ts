import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

const ROLE_ROUTE_PREFIX: Record<string, string> = {
  Admin: "/admin",
  Agent: "/agent",
  Client: "/client",
};

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  const role = request.cookies.get("mockRole")?.value;

  const protectedPrefixes = Object.values(ROLE_ROUTE_PREFIX);
  const isProtectedRoute = protectedPrefixes.some((prefix) => pathname.startsWith(prefix));

  if (!isProtectedRoute) {
    return NextResponse.next();
  }

  if (!role) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  const allowedPrefix = ROLE_ROUTE_PREFIX[role];
  if (!pathname.startsWith(allowedPrefix)) {
    return NextResponse.redirect(new URL(allowedPrefix, request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: ["/admin/:path*", "/agent/:path*", "/client/:path*"],
};