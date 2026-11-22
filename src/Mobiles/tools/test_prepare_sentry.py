import unittest
from prepare_sentry import configuration

class SentryConfigurationTests(unittest.TestCase):
    def test_missing_configuration_is_disabled(self):
        self.assertFalse(configuration({})["SENTRY_ENABLED"])

    def test_explicit_disable_wins(self):
        config = configuration({"SENTRY_DSN": "https://public@example.com/123", "SENTRY_ENABLED": "false"})
        self.assertFalse(config["SENTRY_ENABLED"])

    def test_only_public_settings_are_emitted(self):
        config = configuration({"SENTRY_DSN": "https://public@example.com/123", "SENTRY_AUTH_TOKEN": "private", "SENTRY_RELEASE": "app@sha"})
        self.assertTrue(config["SENTRY_ENABLED"])
        self.assertEqual(config["SENTRY_RELEASE"], "app@sha")
        self.assertNotIn("SENTRY_AUTH_TOKEN", config)
        self.assertNotIn("private", str(config))

    def test_invalid_dsns_are_rejected(self):
        for dsn in ["not-a-dsn", "http://key@example.com/1", "https://example.com/1", "https://key:secret@example.com/1", "https://key@example.com/1?secret=1"]:
            with self.subTest(dsn=dsn), self.assertRaises(ValueError):
                configuration({"SENTRY_DSN": dsn})

if __name__ == "__main__":
    unittest.main()
