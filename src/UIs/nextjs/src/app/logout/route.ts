import { redirect } from "next/navigation";
import { cookies } from "next/headers";

export async function GET() {
  const cookieStore = await cookies();

  // Get environment variables
  const authority =
    process.env.NEXT_PUBLIC_OAUTH_AUTHORITY || "https://localhost:44367";
  const clientId =
    process.env.NEXT_PUBLIC_OAUTH_CLIENT_ID || "ClassifiedAds.React";
  const currentUrl =
    process.env.NEXT_PUBLIC_CURRENT_URL || "http://localhost:3000/";

  // Clear tokens
  cookieStore.delete("access_token");
  cookieStore.delete("refresh_token");

  // Construct logout URL
  const logoutUrl =
    `${authority}/connect/logout?` +
    `post_logout_redirect_uri=${encodeURIComponent(
      currentUrl
    )}?postLogout=true&` +
    `client_id=${encodeURIComponent(clientId)}`;

  redirect(logoutUrl);
}
