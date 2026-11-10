import { router, useLocalSearchParams } from "expo-router";
import { StatusBar } from "expo-status-bar";
import { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  AppState,
  Pressable,
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
    const subscription = AppState.addEventListener("change", (state) =>
      setForeground(state === "active"),
    );
    return () => subscription.remove();
  }, []);
  useEffect(
    () =>
      signedIn && foreground
        ? notifications((message) => {
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
            <HomeButton title="Sign out" onPress={() => setSignOut(true)} />
          </>
        );
    }
  }
  return (
    <SafeAreaView style={s.screen}>
      <StatusBar style={dark ? "light" : "dark"} />
      <View style={s.header}>
        <Text style={s.heading}>CLASSIFIED ADS</Text>
        <Button
          title={`Theme: ${appearance}`}
          onPress={() => setChoosingTheme(true)}
        />
      </View>
      {initializing ? (
        <ActivityIndicator />
      ) : signedIn ? (
        <>
          <ScrollView horizontal style={s.navigation} contentContainerStyle={s.nav} showsHorizontalScrollIndicator={false}>
            {destinations.map(([key, title]) => (
              <Pressable
                key={key}
                accessibilityRole="tab"
                accessibilityState={{ selected: section === key }}
                onPress={() => navigate(key)}
                style={[s.chip, section === key && s.selected]}
              >
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
        <View style={s.body}>
          <Text style={s.title}>Your workspace, anywhere.</Text>
          <Button
            title="Sign in"
            variant="primary"
            disabled={busy}
            onPress={() => void authenticate()}
          />
        </View>
      )}
      {!!error && <Text style={s.error}>{error}</Text>}
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
