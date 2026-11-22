"""Prepare optional public Sentry client configuration. Never writes upload credentials."""
import argparse
import json
import os
from pathlib import Path
from urllib.parse import urlparse

APPS = ("Kotlin", "Flutter", "ReactNative", "MAUI", "Capacitor")

def configuration(env):
    dsn = env.get("SENTRY_DSN", "").strip()
    enabled = env.get("SENTRY_ENABLED", "true").lower() == "true" and bool(dsn)
    if dsn:
        parsed = urlparse(dsn)
        if parsed.scheme != "https" or not parsed.hostname or not parsed.username or parsed.password or not parsed.path.strip("/") or parsed.query or parsed.fragment:
            raise ValueError("SENTRY_DSN must be an HTTPS client DSN with a public key and project ID.")
    return {"SENTRY_ENABLED": enabled, "SENTRY_DSN": dsn,
            "SENTRY_ENVIRONMENT": env.get("SENTRY_ENVIRONMENT", "development"),
            "SENTRY_RELEASE": env.get("SENTRY_RELEASE", "")}

def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--app", choices=APPS, required=True)
    args = parser.parse_args()
    config = configuration(os.environ)
    target = Path(__file__).resolve().parents[1] / args.app / "sentry.config.json"
    target.write_text(json.dumps(config, indent=2) + "\n", encoding="utf-8")
    print(f"{args.app}: Sentry {'configured' if config['SENTRY_ENABLED'] else 'disabled'}")

if __name__ == "__main__":
    main()
