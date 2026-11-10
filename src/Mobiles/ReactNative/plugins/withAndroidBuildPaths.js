const { withAppBuildGradle } = require('expo/config-plugins');

// CMake hashes long object paths before Ninja reaches Windows' path limit.
module.exports = function withAndroidBuildPaths(config) {
  return withAppBuildGradle(config, config => {
    const marker = '// Classified Ads: portable native object paths';
    if (!config.modResults.contents.includes(marker)) {
      config.modResults.contents += `
${marker}
android.defaultConfig.externalNativeBuild.cmake.arguments "-DCMAKE_OBJECT_PATH_MAX=240"
`;
    }
    return config;
  });
};
