import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import { NextRequest } from "next/server";

const generateCodeVerifier = () => {
  const array = new Uint8Array(32);
  crypto.getRandomValues(array);
  return Buffer.from(array).toString("base64url");
};

const generateCodeChallenge = async (verifier: string) => {
  const data = new TextEncoder().encode(verifier);
  const hash = await crypto.subtle.digest("SHA-256", data);
  return Buffer.from(hash).toString("base64url");
};

const generateState = () => {
  const array = new Uint8Array(32);
  crypto.getRandomValues(array);
  return Buffer.from(array).toString("base64url");
};

export async function GET(request: NextRequest) {
  const searchParams = request.nextUrl.searchParams;
  const returnUrl = searchParams.get("returnUrl");

  // Get environment variables
  const authority =
    process.env.NEXT_PUBLIC_OAUTH_AUTHORITY || "https://localhost:44367";
  const clientId =
    process.env.NEXT_PUBLIC_OAUTH_CLIENT_ID || "ClassifiedAds.React";
  const currentUrl =
    process.env.NEXT_PUBLIC_CURRENT_URL || "http://localhost:3000/";

  // Generate PKCE parameters
  const codeVerifier = generateCodeVerifier();
  const codeChallenge = await generateCodeChallenge(codeVerifier);
  const state = generateState();

  // Store verifier and state in secure HTTP-only cookies
  const cookieStore = await cookies();
  cookieStore.set("oauth_code_verifier", codeVerifier, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    maxAge: 60 * 10, // 10 minutes
    path: "/",
  });

  cookieStore.set("oauth_state", state, {
    httpOnly: true,
    secure: process.env.NODE_ENV === "production",
    sameSite: "lax",
    maxAge: 60 * 10, // 10 minutes
    path: "/",
  });

  if (returnUrl) {
    cookieStore.set("oauth_return_url", returnUrl, {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      maxAge: 60 * 10, // 10 minutes
      path: "/",
    });
  }

  // Construct authorization URL
  const params = new URLSearchParams({
    client_id: clientId,
    redirect_uri: `${currentUrl}oidc-login-redirect`,
    response_type: "code",
    scope: "openid profile ClassifiedAds.WebAPI",
    state: state,
    code_challenge: codeChallenge,
    code_challenge_method: "S256",
  });

  const authUrl = `${authority}/connect/authorize?${params.toString()}`;

  redirect(authUrl);
}
