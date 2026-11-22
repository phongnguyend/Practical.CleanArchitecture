import { Component, type ReactNode } from "react";
import { reportFailure } from "../core/crashlytics";
export default class ErrorBoundary extends Component<
  { children: ReactNode },
  { failed: boolean }
> {
  state = { failed: false };
  static getDerivedStateFromError() {
    return { failed: true };
  }
  componentDidCatch(error: Error) {
    void reportFailure("react", error);
  }
  render() {
    if (this.state.failed) {
      return (
        <main className="p-4">
          <h1>Something went wrong</h1>
          <p>Please reload the app and try again.</p>
          <button
            className="btn btn-primary"
            onClick={() => window.location.replace("/home")}
          >
            Reload
          </button>
        </main>
      );
    }
    return this.props.children;
  }
}
