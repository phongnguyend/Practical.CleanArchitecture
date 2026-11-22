#!/bin/sh
set -eu
if [ -f "${SRCROOT}/App/GoogleService-Info.plist" ]; then
  cp "${SRCROOT}/App/GoogleService-Info.plist" "${TARGET_BUILD_DIR}/${UNLOCALIZED_RESOURCES_FOLDER_PATH}/GoogleService-Info.plist"
  if [ "${CONFIGURATION}" = "Release" ] && [ "${PLATFORM_NAME}" = "iphoneos" ]; then
    "${BUILD_DIR%/Build/*}/SourcePackages/checkouts/firebase-ios-sdk/Crashlytics/run"
  fi
fi
cp "${SRCROOT}/App/PrivacyInfo.xcprivacy" "${TARGET_BUILD_DIR}/${UNLOCALIZED_RESOURCES_FOLDER_PATH}/PrivacyInfo.xcprivacy"
