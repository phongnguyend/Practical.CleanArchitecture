import base64
import json
from pathlib import Path
import plistlib
import tempfile
import unittest

from prepare_firebase import APPS, options_for, prepare


def config(platform, identifier):
    if platform == "ios":
        return plistlib.dumps({"BUNDLE_ID": identifier, "API_KEY": "fixture-key",
                              "GOOGLE_APP_ID": "1:123:ios:fixture", "GCM_SENDER_ID": "123", "PROJECT_ID": "fixture"})
    return json.dumps({"project_info": {"project_number": "123", "project_id": "fixture"},
                       "client": [{"client_info": {"android_client_info": {"package_name": identifier},
                                                   "mobilesdk_app_id": "1:123:android:fixture"},
                                   "api_key": [{"current_key": "fixture-key"}]}]}).encode()


class FirebaseConfigurationTests(unittest.TestCase):
    def test_all_seven_app_registrations_accept_matching_config(self):
        for app, platforms in APPS.items():
            for platform, (identifier, relative) in platforms.items():
                with self.subTest(app=app, platform=platform), tempfile.TemporaryDirectory() as folder:
                    content = config(platform, identifier)
                    self.assertTrue(prepare(app, platform, root=Path(folder), encoded=base64.b64encode(content)))
                    self.assertEqual((Path(folder) / app / relative).read_bytes(), content)

    def test_wrong_app_is_rejected_without_writing_config(self):
        for platform in ("android", "ios"):
            with self.subTest(platform=platform), tempfile.TemporaryDirectory() as folder:
                with self.assertRaises(ValueError):
                    prepare("ReactNative", platform, root=Path(folder), encoded=base64.b64encode(config(platform, "wrong.app")))
                self.assertEqual(list(Path(folder).iterdir()), [])

    def test_absent_configuration_is_disabled(self):
        with tempfile.TemporaryDirectory() as folder:
            root = Path(folder)
            self.assertFalse(prepare("Flutter", "android", root=root))
            self.assertEqual(json.loads((root / "Flutter/firebase-defines.json").read_text()), {"CRASHLYTICS_ENABLED": False})
            with self.assertRaises(ValueError):
                prepare("MAUI", "ios", root=root, required=True)

    def test_corrupt_base64_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder, self.assertRaises(ValueError):
            prepare("Kotlin", "android", root=Path(folder), encoded="!!invalid!!")

    def test_empty_supplied_file_is_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            source = Path(folder) / "empty.json"
            source.write_bytes(b"")
            with self.assertRaises(ValueError):
                prepare("Kotlin", "android", source=source, root=Path(folder))

    def test_required_values_cannot_be_empty(self):
        data = json.loads(config("android", "expected.app"))
        data["project_info"]["project_id"] = ""
        with self.assertRaises(ValueError):
            options_for(json.dumps(data).encode(), "android", "expected.app")


if __name__ == "__main__":
    unittest.main()
