import { Stack } from "expo-router";
import { useEffect } from "react";
import { initializeCrashReporting } from "../core/diagnostics/crash-reporting";
export default function Layout() {
  useEffect(() => {
    void initializeCrashReporting().catch(() => {
      console.warn("Crash reporting could not be initialized.");
    });
  }, []);
  return <Stack screenOptions={{ headerShown: false }} />;
}
