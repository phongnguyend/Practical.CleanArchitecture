const { withDangerousMod, XML } = require('expo/config-plugins');
const fs = require('node:fs/promises');
const path = require('node:path');
const { X509Certificate } = require('node:crypto');

// Native resources belong only to src/debug, never to release builds.
module.exports = function withDebugCertificate(config) {
  return withDangerousMod(config, ['android', async config => {
    const debug = path.join(config.modRequest.platformProjectRoot, 'app/src/debug');
    const manifestPath = path.join(debug, 'AndroidManifest.xml');
    const manifest = await XML.parseXMLAsync(await fs.readFile(manifestPath, 'utf8'));
    const application = manifest.manifest.application[0].$;
    const certificatePath = process.env.MOBILE_DEBUG_CERTIFICATE;
    const resource = path.join(debug, 'res/raw/local_development_certificate.cer');
    const security = path.join(debug, 'res/xml/local_certificate_security_config.xml');
    if (certificatePath) {
      const certificate = new X509Certificate(await fs.readFile(path.resolve(certificatePath)));
      await fs.mkdir(path.dirname(resource), { recursive: true });
      await fs.mkdir(path.dirname(security), { recursive: true });
      await fs.writeFile(resource, certificate.raw);
      await fs.writeFile(security, `<network-security-config>
  <base-config cleartextTrafficPermitted="true"><trust-anchors><certificates src="system" /></trust-anchors></base-config>
  <domain-config cleartextTrafficPermitted="true">
    <domain>localhost</domain>
    <trust-anchors><certificates src="system" /><certificates src="@raw/local_development_certificate" /></trust-anchors>
  </domain-config>
</network-security-config>`);
      // Preserve Expo's debug HTTP Metro access; configured API/issuer URLs use HTTPS.
      application['android:networkSecurityConfig'] = '@xml/local_certificate_security_config';
    } else {
      if (application['android:networkSecurityConfig'] === '@xml/local_certificate_security_config') {
        delete application['android:networkSecurityConfig'];
      }
      await fs.rm(resource, { force: true });
      await fs.rm(security, { force: true });
    }
    await fs.writeFile(manifestPath, XML.format(manifest));
    return config;
  }]);
};
