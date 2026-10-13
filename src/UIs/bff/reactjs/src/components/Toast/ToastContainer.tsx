"use client";
import { useEffect } from "react";
import { mountToasts } from "./toast";
import "./toast.css";
export { toast } from "./toast";
export type { ToastOptions, ToastKind } from "./toast";

export default function ToastContainer() {
  useEffect(() => mountToasts(), []);
  return null;
}
