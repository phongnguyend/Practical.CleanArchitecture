import { sessionUnlocked, biometricEnabled, biometricAvailable, enableBiometrics, disableBiometrics, unlockSession, lockSession } from "../../core/auth/session";
import { router, useLocalSearchParams } from "expo-router";
import { StatusBar } from "expo-status-bar";
import { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  AppState,
  Pressable,
  Image,
  ScrollView,
  Text,
  View,
  useColorScheme,
} from "react-native";
import { SafeAreaProvider, SafeAreaView } from "react-native-safe-area-context";
import {
  Appearance,
  login,
  logout,
  notifications,
  readTheme,
  restore,
  saveTheme,
} from "../../core/services";
import { HomeInfo } from "../../shared/ui/record-ui";
import { actionIcons } from "../../shared/ui/action-icons";
import { Button, HomeButton, Sheet, Theme, useStyles } from "../../shared/ui/ui";
import AuditLogsScreen from "../auditlogs/AuditLogsScreen";
import ConfigurationEntryScreen from "../configurationentries/ConfigurationEntryScreen";
import StoredFileScreen from "../files/StoredFileScreen";
import ProductScreen from "../products/ProductScreen";
import RoleScreen from "../roles/RoleScreen";
import UserScreen from "../users/UserScreen";
const destinations = [
  ["home", "Home"],
  ["products", "Products"],
  ["files", "Files"],
  ["users", "Users"],
  ["roles", "Roles"],
  ["configurationentries", "Configuration Entries"],
  ["auditlogentries", "Audit logs"],
  ["notifications", "Notifications"],
];
export default function App() {
  const system = useColorScheme();
  const [appearance, setAppearance] = useState<Appearance>("system");
  useEffect(() => {
    void readTheme().then(setAppearance);
  }, []);
  return (
    <SafeAreaProvider>
      <Theme.Provider
        value={
          appearance === "dark" ||
          (appearance === "system" && system === "dark")
        }
      >
        <Workspace
          appearance={appearance}
          chooseTheme={(value) => {
            setAppearance(value);
            void saveTheme(value);
          }}
        />
      </Theme.Provider>
    </SafeAreaProvider>
  );
}
function Workspace({
  appearance,
  chooseTheme,
}: {
  appearance: Appearance;
  chooseTheme: (value: Appearance) => void;
}) {
  const s = useStyles();
  const dark = useContext(Theme);
  const route = useLocalSearchParams<{ section: string }>();
  const section = destinations.some(([key]) => key === route.section)
    ? route.section
    : "home";
  const [signedIn, setSignedIn] = useState(false),
    [initializing, setInitializing] = useState(true),
    [busy, setBusy] = useState(false),
    [error, setError] = useState(""),
    [choosingTheme, setChoosingTheme] = useState(false),
    [signOut, setSignOut] = useState(false),
    [feed, setFeed] = useState<string[]>([]),
    [status, setStatus] = useState("Connecting…"),
    [toast, setToast] = useState(""),
    [foreground, setForeground] = useState(true);
  useEffect(() => {
    restore()
      .then(setSignedIn)
      .catch((e) => setError(String(e)))
      .finally(() => setInitializing(false));
  }, []);
  useEffect(() => {
    const subscription = AppState.addEventListener("change", (state) => {
      setForeground(state === "active");
      if (state === "background" && lockSession()) {
        setSignedIn(false);
        setFeed([]);
        setToast("");
      }
    });
    return () => subscription.remove();
  }, []);
  useEffect(
    () =>
      signedIn && foreground
        ? notifications((message) => {
            if (!sessionUnlocked()) {
              return;
            }
            setToast(message);
            setFeed((items) => [message, ...items].slice(0, 100));
          }, setStatus)
        : undefined,
    [signedIn, foreground],
  );
  useEffect(() => {
    if (!toast) return;
    const timer = setTimeout(() => setToast(""), 6000);
    return () => clearTimeout(timer);
  }, [toast]);
  function navigate(key: string) {
    if (key !== section) router.setParams({ section: key });
  }
  async function authenticate(out = false) {
    setBusy(true);
    setError("");
    try {
      if (out) {
        setSignedIn(false);
        setFeed([]);
        navigate("home");
        await logout();
      } else {
        await login();
        setSignedIn(true);
      }
    } catch (e) {
      setError(String(e));
    } finally {
      setBusy(false);
    }
  }
  async function biometricAction(action: () => Promise<unknown>) {
    setBusy(true);
    setError("");
    try {
      await action();
    } catch {
      setError("Biometric authentication did not complete. Try again or sign in with Identity Server.");
    } finally {
      setBusy(false);
    }
  }
  function screen() {
    switch (section) {
      case "products":
        return <ProductScreen />;
      case "files":
        return <StoredFileScreen />;
      case "users":
        return <UserScreen />;
      case "roles":
        return <RoleScreen />;
      case "configurationentries":
        return <ConfigurationEntryScreen />;
      case "auditlogentries":
        return <AuditLogsScreen />;
      case "notifications":
        return (
          <>
            <Text accessibilityRole="header" style={s.sectionTitle}>Notifications</Text>
            <Text style={s.text}>{status}</Text>
            {!feed.length && (
              <Text style={s.muted}>
                Live messages appear here while the app is active.
              </Text>
            )}
            {feed.map((message, index) => (
              <Text key={index} style={s.text}>
                {message}
              </Text>
            ))}
          </>
        );
      default:
        return (
          <>
            <HomeInfo />
            {destinations.slice(1, 6).map(([key, title]) => (
              <HomeButton key={key} title={title} onPress={() => navigate(key)} />
            ))}
            {(biometricEnabled() || biometricAvailable()) && <HomeButton disabled={busy}
              title={biometricEnabled() ? "Disable biometric login" : "Enable biometric login"}
              onPress={() => void biometricAction(biometricEnabled() ? disableBiometrics : enableBiometrics)} />}
            {biometricEnabled() && <HomeButton title="Lock app" onPress={() => {
              lockSession();
              setSignedIn(false);
              setFeed([]);
              setToast("");
            }} />}
            <HomeButton disabled={busy} title="Sign out" onPress={() => setSignOut(true)} />
          </>
        );
    }
  }
  return (
    <SafeAreaView style={s.screen}>
      <StatusBar style={dark ? "light" : "dark"} />
      <View style={s.header}>
        <Text style={s.heading}>CLASSIFIED ADS (React Native)</Text>
        <Button
          title={`Theme: ${appearance}`}
          icon="Theme"
          iconOnly
          onPress={() => setChoosingTheme(true)}
        />
      </View>
      {initializing ? (
        <ActivityIndicator />
      ) : signedIn ? (
        <>
          <ScrollView horizontal style={s.navigation} contentContainerStyle={s.nav} showsHorizontalScrollIndicator={false} keyboardShouldPersistTaps="handled">
            {destinations.map(([key, title]) => (
              <Pressable
                key={key}
                accessibilityRole="tab"
                accessibilityLabel={title}
                accessibilityState={{ selected: section === key }}
                onPress={() => navigate(key)}
                style={[s.chip, section === key && s.selected, { flexDirection: "row", alignItems: "center", gap: 8 }]}
              >
                <Image source={actionIcons[title]} accessible={false} style={{ width: 18, height: 18, tintColor: section === key ? s.buttonText.color : s.text.color }} />
                <Text style={section === key ? s.buttonText : s.text}>
                  {title}
                </Text>
              </Pressable>
            ))}
          </ScrollView>
          <ScrollView
            key={section}
            contentContainerStyle={s.body}
            keyboardShouldPersistTaps="handled"
          >
            {screen()}
          </ScrollView>
        </>
      ) : (
        <ScrollView contentContainerStyle={s.body}>
          <Text style={s.title}>{biometricEnabled() ? "Workspace locked" : "Your workspace, anywhere."}</Text>
          {biometricEnabled() && <Button title="Unlock with biometrics" icon="Set password" disabled={busy}
            onPress={() => void biometricAction(async () => { await unlockSession(); setSignedIn(true); })} />}
          {biometricEnabled() && <Text style={s.muted}>Or sign in with Identity Server to recover access or switch accounts.</Text>}
          <Button
            title="Sign in"
            variant="primary"
            disabled={busy}
            onPress={() => void authenticate()}
          />
        </ScrollView>
      )}
      {!!error && <View style={[s.card, { marginHorizontal: 16, marginBottom: 12, flexDirection: "row", alignItems: "center" }]}>
        <Text accessibilityRole="alert" style={[s.error, { flex: 1 }]}>{error}</Text>
        <Button title="Dismiss error" icon="Close" iconOnly onPress={() => setError("")} />
      </View>}
      {!!toast && (
        <Text accessibilityRole="alert" style={s.toast}>
          {toast}
        </Text>
      )}
      {choosingTheme && (
        <Sheet
          title="Appearance"
          close={() => setChoosingTheme(false)}
          footer={
            <Button title="Done" onPress={() => setChoosingTheme(false)} />
          }
        >
          {(["system", "light", "dark"] as Appearance[]).map((value) => (
            <Button
              key={value}
              title={value}
              onPress={() => {
                chooseTheme(value);
                setChoosingTheme(false);
              }}
            />
          ))}
        </Sheet>
      )}
      {signOut && (
        <Sheet
          title="Confirm action"
          close={() => setSignOut(false)}
          footer={
            <>
              <Button title="Cancel" onPress={() => setSignOut(false)} />
              <Button
                title="Confirm"
                onPress={() => {
                  setSignOut(false);
                  void authenticate(true);
                }}
              />
            </>
          }
        >
          <Text style={s.text}>Sign out of this device?</Text>
        </Sheet>
      )}
    </SafeAreaView>
  );
}
