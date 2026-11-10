const { test } = require("node:test");
const assert = require("node:assert/strict");
const fs = require("node:fs");
const path = require("node:path");
const Module = require("node:module");
const ts = require("typescript");
const cache = new Map();
function load(relativePath) {
  const filename = path.resolve(__dirname, relativePath);
  if (cache.has(filename)) return cache.get(filename).exports;
  const compiled = new Module(filename, module);
  compiled.filename = filename;
  compiled.paths = module.paths;
  cache.set(filename, compiled);
  const nativeRequire = compiled.require.bind(compiled);
  compiled.require = (name) =>
    name.startsWith(".")
      ? load(path.resolve(path.dirname(filename), name + ".ts"))
      : nativeRequire(name);
  compiled._compile(
    ts.transpileModule(fs.readFileSync(filename, "utf8"), {
      compilerOptions: {
        module: ts.ModuleKind.CommonJS,
        esModuleInterop: true,
      },
    }).outputText,
    filename,
  );
  return compiled.exports;
}
const products = load("../src/features/products/model.ts");
const files = load("../src/features/files/model.ts");
const users = load("../src/features/users/model.ts");
const roles = load("../src/features/roles/model.ts");
const configuration = load("../src/features/configurationentries/model.ts");
const { auditSnapshot, apiError } = load("../src/shared/models/record.ts");
test("Product validation and payload follow the product API", () => {
  const product = {
    ...products.emptyProduct(),
    name: "ab",
    code: "abc",
    description: "Example",
  };
  assert.match(products.validateProduct(product), /at least 3/);
  product.name = "Sample";
  assert.equal(products.validateProduct(product), undefined);
  assert.deepEqual(products.productsPayload(product), {
    name: "Sample",
    code: "abc",
    description: "Example",
  });
  product.code = "12345678901";
  assert.match(products.validateProduct(product), /at most 10/);
});
test("File validation requires upload metadata and preserves encryption", () => {
  const file = {
    ...files.emptyStoredFile(),
    name: "Report",
    description: "",
    encrypted: true,
  };
  assert.match(files.validateStoredFile(file), /required/);
  file.description = "Summary";
  assert.equal(files.validateStoredFile(file), undefined);
  assert.equal(files.filesPayload(file).encrypted, true);
  assert.equal(
    files.parseStoredFile({ id: "1", fileName: "report.pdf", encrypted: true })
      .fileName,
    "report.pdf",
  );
});
test("User payload trims email, converts dates and retains all account fields", () => {
  const user = {
    ...users.emptyUser(),
    id: "u1",
    userName: "tester",
    email: " test@example.com ",
    accessFailedCount: 2,
    emailConfirmed: true,
  };
  assert.equal(users.validateUser(user), undefined);
  const body = users.usersPayload(user);
  assert.equal(body.email, "test@example.com");
  assert.equal(body.lockoutEnd, null);
  assert.equal(body.accessFailedCount, 2);
  assert.equal(body.id, "u1");
  user.accessFailedCount = -1;
  assert.match(users.validateUser(user), /nonnegative/);
  user.accessFailedCount = 0;
  user.email = "invalid";
  assert.match(users.validateUser(user), /email/);
});
test("Role and configuration validation stay feature-specific", () => {
  assert.match(roles.validateRole(roles.emptyRole()), /required/);
  const setting = {
    ...configuration.emptyConfigurationEntry(),
    key: "Setting",
    value: "  exact value  ",
    isSensitive: true,
  };
  assert.equal(configuration.validateConfigurationEntry(setting), undefined);
  assert.equal(
    configuration.configurationentriesPayload(setting).value,
    "  exact value  ",
  );
});
test("Audit and API helpers handle snapshots and validation errors", () => {
  assert.deepEqual(auditSnapshot({ data: { name: "Updated" } }), {
    name: "Updated",
  });
  assert.deepEqual(auditSnapshot({ log: '{"name":"Created"}' }), {
    name: "Created",
  });
  assert.deepEqual(auditSnapshot({ log: "invalid" }), {});
  assert.equal(
    apiError(400, '{"errors":{"Name":["Name is required"]}}'),
    "Name is required",
  );
});
