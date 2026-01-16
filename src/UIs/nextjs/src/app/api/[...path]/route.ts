import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";
import https from "https";
import axios from "axios";
import { decrypt } from "@/lib/crypto";

// Create axios instance with custom HTTPS agent for development
const httpsAgent = new https.Agent({
  rejectUnauthorized: process.env.NODE_ENV === "production",
});

const axiosInstance = axios.create({
  httpsAgent,
  validateStatus: () => true, // Don't throw on any status code
  maxRedirects: 5,
  timeout: 30000,
});

async function handleRequest(request: NextRequest, params: { path: string[] }) {
  const cookieStore = await cookies();
  const encryptedToken = cookieStore.get("access_token")?.value;

  // Decrypt the access token
  let accessToken: string | undefined;
  if (encryptedToken) {
    try {
      accessToken = await decrypt(encryptedToken);
    } catch (error) {
      console.error("Failed to decrypt access token:", error);
      accessToken = undefined;
    }
  }

  // Get the resource server endpoint
  const resourceServerBase =
    process.env.NEXT_PUBLIC_RESOURCE_SERVER_ENDPOINT ||
    "https://localhost:44312";

  // Build the target URL
  const path = params.path?.join("/") || "";
  const searchParams = request.nextUrl.searchParams.toString();
  const targetUrl = `${resourceServerBase}/api/${path}${
    searchParams ? `?${searchParams}` : ""
  }`;

  console.log(`Proxying ${request.method} request to:`, targetUrl);

  // Prepare headers
  const headers: any = {};

  // Copy relevant headers from the original request
  request.headers.forEach((value, key) => {
    // Skip host and connection headers
    if (!["host", "connection", "content-length"].includes(key.toLowerCase())) {
      headers[key] = value;
    }
  });

  // Add authorization header
  if (accessToken) {
    headers["Authorization"] = `Bearer ${accessToken}`;
  }

  // Prepare request body for methods that support it
  let body = undefined;
  if (["POST", "PUT", "PATCH"].includes(request.method)) {
    const contentType = request.headers.get("content-type");
    if (contentType?.includes("application/json")) {
      body = await request.json();
    } else {
      body = await request.text();
    }
  }

  try {
    const response = await axiosInstance({
      method: request.method,
      url: targetUrl,
      headers,
      data: body,
    });

    // Create response with same status and headers
    const responseHeaders = new Headers();
    Object.entries(response.headers).forEach(([key, value]) => {
      // Skip headers that shouldn't be forwarded
      if (
        !["content-encoding", "transfer-encoding", "connection"].includes(
          key.toLowerCase()
        )
      ) {
        responseHeaders.set(key, value as string);
      }
    });

    return new NextResponse(
      typeof response.data === "string"
        ? response.data
        : JSON.stringify(response.data),
      {
        status: response.status,
        statusText: response.statusText,
        headers: responseHeaders,
      }
    );
  } catch (error) {
    console.error("Failed to proxy", targetUrl, error);
    return NextResponse.json(
      { error: "Failed to proxy request to resource server" },
      { status: 502 }
    );
  }
}

export async function GET(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  return handleRequest(request, await params);
}

export async function POST(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  return handleRequest(request, await params);
}

export async function PUT(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  return handleRequest(request, await params);
}

export async function DELETE(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  return handleRequest(request, await params);
}

export async function PATCH(
  request: NextRequest,
  { params }: { params: Promise<{ path: string[] }> }
) {
  return handleRequest(request, await params);
}
