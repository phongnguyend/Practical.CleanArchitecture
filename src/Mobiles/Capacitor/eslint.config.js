import tseslint from "typescript-eslint";
export default tseslint.config({
  files: ["**/*.{ts,tsx}"],
  languageOptions: { parser: tseslint.parser, parserOptions: { ecmaFeatures: { jsx: true } } },
  rules: { curly: ["error", "all"] },
});
