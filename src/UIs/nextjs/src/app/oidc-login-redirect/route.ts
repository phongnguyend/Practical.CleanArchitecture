import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import { NextRequest, NextResponse } from "next/server";
import { encrypt } from "@/lib/crypto";

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams;
  const code = searchParams.get("code");
  const state = searchParams.get("state");

  if (!code || !state) {
    redirect("/login?error=missing_parameters");
  }

  const cookieStore = await cookies();

  // Verify state
  const storedState = cookieStore.get("oauth_state")?.value;
  if (!storedState || storedState !== state) {
    redirect("/login?error=invalid_state");
  }

  // Get code verifier
  const codeVerifier = cookieStore.get("oauth_code_verifier")?.value;
  if (!codeVerifier) {
    redirect("/login?error=missing_verifier");
  }

  // Get environment variables
  const authority =
    process.env.NEXT_PUBLIC_OAUTH_AUTHORITY || "https://localhost:44367";
  const clientId =
    process.env.NEXT_PUBLIC_OAUTH_CLIENT_ID || "ClassifiedAds.React";
  const currentUrl =
    process.env.NEXT_PUBLIC_CURRENT_URL || "http://localhost:3000/";

  // Exchange code for tokens
  const tokenEndpoint = `${authority}/connect/token`;
  const tokenParams = new URLSearchParams({
    grant_type: "authorization_code",
    code: code,
    redirect_uri: `${currentUrl}oidc-login-redirect`,
    client_id: clientId,
    code_verifier: codeVerifier,
  });

  try {
    // In development, disable SSL verification for self-signed certificates
    const fetchOptions: RequestInit = {
      method: "POST",
      headers: {
        "Content-Type": "application/x-www-form-urlencoded",
      },
      body: tokenParams.toString(),
    };

    const tokenResponse = await fetch(tokenEndpoint, fetchOptions);

    if (!tokenResponse.ok) {
      const errorText = await tokenResponse.text();
      console.error("Token exchange failed:", errorText);
      redirect("/login?error=token_exchange_failed");
    }

    const tokens = await tokenResponse.json();

    // Encrypt and store access token in secure HTTP-only cookie
    const encryptedAccessToken = await encrypt(tokens.access_token);
    cookieStore.set("access_token", encryptedAccessToken, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      maxAge: tokens.expires_in || 3600, // Use token expiry or default to 1 hour
      path: "/",
    });

    // Encrypt and store refresh token if provided
    if (tokens.refresh_token) {
      const encryptedRefreshToken = await encrypt(tokens.refresh_token);
      cookieStore.set("refresh_token", encryptedRefreshToken, {
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        sameSite: "lax",
        maxAge: 60 * 60 * 24 * 30, // 30 days
        path: "/",
      });
    }

    // Clean up OAuth flow cookies
    cookieStore.delete("oauth_code_verifier");
    cookieStore.delete("oauth_state");

    // Get return URL
    const returnUrl = cookieStore.get("oauth_return_url")?.value || "/";
    cookieStore.delete("oauth_return_url");

    // Return redirect response instead of using redirect()
    return NextResponse.redirect(new URL(returnUrl, request.url));
  } catch (error) {
    console.error("OAuth callback error:", error);
    return NextResponse.redirect(
      new URL("/login?error=oauth_callback_failed", request.url)
    );
  }
}
