import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";

export function middleware(request: NextRequest) {
  // Get the access token from cookies
  const accessToken = request.cookies.get("access_token")?.value;

  // Clone the request headers
  const requestHeaders = new Headers(request.headers);

  if (!accessToken) {
    // Return 401 for API requests without auth
    return NextResponse.json({ error: "Unauthorized" }, { status: 401 });
  }

  return NextResponse.next({
    request: {
      headers: requestHeaders,
    },
  });
}

export const config = {
  matcher: ["/api/:path*"],
};
