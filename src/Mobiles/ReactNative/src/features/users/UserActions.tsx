import { useState } from "react";
import { Text } from "react-native";
import { Input } from "../../shared/ui/fields";
import { Button, Sheet, useStyles } from "../../shared/ui/ui";
import { sendConfirmationEmail, sendResetEmail, setPassword } from "./api";
import type { User } from "./model";
export function validatePassword(password: string, confirmation: string) {
  return !password.trim()
    ? "New password is required."
    : password !== confirmation
      ? "Passwords do not match."
      : undefined;
}
export function UserActions({
  value,
  notify,
}: {
  value: User;
  notify: (text: string) => void;
}) {
  const s = useStyles();
  const [mode, setMode] = useState<"password" | "reset" | "confirm" | null>(
      null,
    ),
    [password, changePassword] = useState(""),
    [confirmation, changeConfirmation] = useState(""),
    [error, setError] = useState(""),
    [busy, setBusy] = useState(false),
    [discard, setDiscard] = useState(false);
  function open(next: typeof mode) {
    setError("");
    changePassword("");
    changeConfirmation("");
    setDiscard(false);
    setMode(next);
  }
  function close() {
    if (busy) return;
    if (mode === "password" && (password || confirmation)) setDiscard(true);
    else setMode(null);
  }
  async function save() {
    setError("");
    if (mode === "password") {
      const message = validatePassword(password, confirmation);
      if (message) {
        setError(message);
        return;
      }
    }
    setBusy(true);
    try {
      if (mode === "password") await setPassword(value.id, password);
      else if (mode === "reset") await sendResetEmail(value.id);
      else await sendConfirmationEmail(value.id);
      notify(mode === "password" ? "Password updated" : "Email requested");
      setMode(null);
    } catch (e) {
      setError(String(e));
    } finally {
      setBusy(false);
    }
  }
  return (
    <>
      <Button title="Set password" onPress={() => open("password")} />
      <Button title="Reset email" onPress={() => open("reset")} />
      <Button title="Confirm email" onPress={() => open("confirm")} />
      {mode && (
        <Sheet
          title={
            discard
              ? "Discard unsaved changes?"
              : mode === "password"
                ? "Set password"
                : "Confirm action"
          }
          close={close}
          footer={
            discard ? (
              <>
                <Button
                  title="Keep editing"
                  onPress={() => setDiscard(false)}
                />
                <Button title="Discard" onPress={() => setMode(null)} />
              </>
            ) : (
              <>
                <Button title="Cancel" disabled={busy} onPress={close} />
                <Button
                  title={mode === "password" ? "Save" : "Confirm"}
                  disabled={busy}
                  onPress={() => void save()}
                />
              </>
            )
          }
        >
          {discard ? (
            <Text style={s.text}>Discard unsaved changes?</Text>
          ) : mode === "password" ? (
            <>
              <Input
                label="New password"
                value={password}
                onChange={changePassword}
                secret
                disabled={busy}
              />
              <Input
                label="Confirm password"
                value={confirmation}
                onChange={changeConfirmation}
                secret
                disabled={busy}
              />
            </>
          ) : (
            <Text style={s.text}>
              Send {mode === "reset" ? "password reset" : "confirmation"} email
              to {value.email}?
            </Text>
          )}
          {!!error && (
            <Text accessibilityRole="alert" style={s.error}>
              {error}
            </Text>
          )}
        </Sheet>
      )}
    </>
  );
}
