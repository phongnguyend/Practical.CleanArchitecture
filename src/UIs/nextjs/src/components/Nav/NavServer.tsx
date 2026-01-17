import { cookies } from "next/headers";
import Nav from "./Nav";

export default async function NavServer() {
  const cookieStore = await cookies();
  const isAuthenticated = !!cookieStore.get("access_token")?.value;

  return <Nav isAuthenticated={isAuthenticated} />;
}
