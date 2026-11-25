"""Validate Firebase client configuration and prepare one mobile build (Python 3)."""
import argparse
import base64
import json
import os
from pathlib import Path
import plistlib

ROOT = Path(__file__).resolve().parents[1]
APPS = {
    "Kotlin": {"android": ("com.classifiedads.mobile.kotlin", "app/google-services.json")},
    "ReactNative": {
        "android": ("com.classifiedads.mobile.reactnative", "google-services.json"),
        "ios": ("com.classifiedads.mobile.reactnative", "GoogleService-Info.plist"),
    },
    "Flutter": {
        "android": ("com.classifiedads.mobile.flutter", "android/app/google-services.json"),
        "ios": ("com.classifiedads.mobile.flutter", "ios/Runner/GoogleService-Info.plist"),
    },
    "Capacitor": {
        "android": ("com.classifiedads.mobile.capacitor", "android/app/google-services.json"),
        "ios": ("com.classifiedads.mobile.capacitor", "ios/App/App/GoogleService-Info.plist"),
    },
    "MAUI": {
        "android": ("com.classifiedads.mobile.maui", "App/google-services.json"),
        "ios": ("com.classifiedads.mobile.maui", "App/GoogleService-Info.plist"),
    },
}


def options_for(content, platform, identifier):
    if platform == "android":
        data = json.loads(content)
        clients = [client for client in data["client"]
                   if client["client_info"]["android_client_info"]["package_name"] == identifier]
        if len(clients) != 1:
            raise ValueError("Firebase Android config must contain exactly one matching app: " + identifier)
        client = clients[0]
        result = {
            "FIREBASE_API_KEY": client["api_key"][0]["current_key"],
            "FIREBASE_APP_ID": client["client_info"]["mobilesdk_app_id"],
            "FIREBASE_SENDER_ID": data["project_info"]["project_number"],
            "FIREBASE_PROJECT_ID": data["project_info"]["project_id"],
        }
    else:
        data = plistlib.loads(content)
        if data["BUNDLE_ID"] != identifier:
            raise ValueError("Firebase iOS bundle ID must be " + identifier)
        result = {target: data[source] for target, source in {
            "FIREBASE_API_KEY": "API_KEY", "FIREBASE_APP_ID": "GOOGLE_APP_ID",
            "FIREBASE_SENDER_ID": "GCM_SENDER_ID", "FIREBASE_PROJECT_ID": "PROJECT_ID",
            "FIREBASE_BUNDLE_ID": "BUNDLE_ID",
        }.items()}
    if not all(isinstance(value, str) and value.strip() for value in result.values()):
        raise ValueError("Firebase configuration has an empty or invalid required value")
    result["CRASHLYTICS_ENABLED"] = True
    return result


def prepare(app, platform, source=None, root=ROOT, encoded=None, required=False, mobile_push=False):
    identifier, relative = APPS[app][platform]
    target = root / app / relative
    content = (source.read_bytes() if source else
               base64.b64decode(encoded, validate=True) if encoded else
               target.read_bytes() if target.is_file() else None)
    if content is None and (required or mobile_push):
        raise ValueError("Firebase configuration is required for this build")
    options = options_for(content, platform, identifier) if content is not None else {"CRASHLYTICS_ENABLED": False}
    options["PUSH_NOTIFICATIONS_ENABLED"] = mobile_push
    if content is not None:
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(content)
    if app == "Flutter":
        defines = root / app / "firebase-defines.json"
        defines.parent.mkdir(parents=True, exist_ok=True)
        defines.write_text(json.dumps(options, indent=2) + "\n", encoding="utf-8")
    if app == "ReactNative":
        defines = root / app / "push.config.json"
        defines.parent.mkdir(parents=True, exist_ok=True)
        defines.write_text(json.dumps({"enabled": mobile_push}) + "\n", encoding="utf-8")
    return content is not None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--app", required=True, choices=APPS)
    parser.add_argument("--platform", required=True, choices=("android", "ios"))
    parser.add_argument("--config", type=Path)
    parser.add_argument("--require-config", action="store_true")
    args = parser.parse_args()
    mobile_push = os.environ.get("MOBILE_PUSH_ENABLED", "false").lower() == "true"
    try:
        enabled = prepare(args.app, args.platform, args.config,
                          encoded=os.environ.get("FIREBASE_CONFIG_BASE64"), required=args.require_config, mobile_push=mobile_push)
    except (ValueError, KeyError, IndexError, OSError, plistlib.InvalidFileException):
        # Do not print file contents or parser exceptions containing configuration values.
        parser.exit(1, "Invalid or missing Firebase configuration for the selected application/platform.\n")
    if os.environ.get("GITHUB_ENV"):
        with open(os.environ["GITHUB_ENV"], "a", encoding="utf-8") as output:
            output.write("FIREBASE_ENABLED=" + str(enabled).lower() + "\n")
            for key in ("MOBILE_PUSH_ENABLED", "PUSH_NOTIFICATIONS_ENABLED", "VITE_PUSH_NOTIFICATIONS_ENABLED"):
                output.write(key + "=" + str(mobile_push).lower() + "\n")
    print(args.app + " " + args.platform + ": Crashlytics " + ("configured" if enabled else "disabled (no configuration)"))


if __name__ == "__main__":
    main()
