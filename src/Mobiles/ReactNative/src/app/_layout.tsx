import { initializeSentry, wrapWithSentry } from "../core/diagnostics/sentry";
import { Stack } from "expo-router";
import { useEffect } from "react";
import { initializeCrashReporting } from "../core/diagnostics/crash-reporting";
initializeSentry();

function Layout() {
  useEffect(() => {
    void initializeCrashReporting().catch(() => {
      console.warn("Crash reporting could not be initialized.");
    });
  }, []);
  return <Stack screenOptions={{ headerShown: false }} />;
}

export default wrapWithSentry(Layout);
