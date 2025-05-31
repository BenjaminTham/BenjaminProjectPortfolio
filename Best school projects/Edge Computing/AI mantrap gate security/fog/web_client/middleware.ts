import {NextRequest, NextResponse} from "next/server";

const authenticatedRoutes: {[index: string]: string[]} = {
  "/": ["admin", "security"],
  "/users": ["admin"],
  "/mantraps": ["admin", "security"],
  "/logs": ["admin", "security"]
}

const getCookie = (req: NextRequest, key: string) => {
  return JSON.parse(req.cookies.get(key)?.value ?? "null")
}

export async function middleware(req: NextRequest) {
  const path = req.nextUrl.pathname
  // Redirect to home is logged in
  if (path === "/login" && getCookie(req, "user") !== null)
    return NextResponse.redirect(new URL("/", req.url))
  // Redirect to login if protected route but no permission
  const allowedRoles = authenticatedRoutes[path]
  if (allowedRoles && !allowedRoles.includes(getCookie(req, "user")?.role))
    return NextResponse.redirect(new URL("/login", req.url))
  return NextResponse.next()
}