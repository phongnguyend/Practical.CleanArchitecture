import Image from "next/image";
import styles from "./page.module.css";
import OidcLoginRedirect from "@/containers/Auth/OidcLoginRedirect";

export default function Page() {
  return <OidcLoginRedirect />;
}
