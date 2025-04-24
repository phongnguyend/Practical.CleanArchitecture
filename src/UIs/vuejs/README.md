# test

This template should help get you started developing with Vue 3 in Vite.

## Recommended IDE Setup

[VSCode](https://code.visualstudio.com/) + [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) (and disable Vetur).

## Type Support for `.vue` Imports in TS

TypeScript cannot handle type information for `.vue` imports by default, so we replace the `tsc` CLI with `vue-tsc` for type checking. In editors, we need [Volar](https://marketplace.visualstudio.com/items?itemName=Vue.volar) to make the TypeScript language service aware of `.vue` types.

## Customize configuration

See [Vite Configuration Reference](https://vite.dev/config/).

## Project Setup

```sh
npm install
```

### Compile and Hot-Reload for Development

```sh
npm run dev
```

### Type-Check, Compile and Minify for Production

```sh
npm run build
```

### Lint with [ESLint](https://eslint.org/)

```sh
npm run lint
```

### SonarQube

```
echo %SONAR_TOKEN%
setx SONAR_TOKEN <token>
set SONAR_TOKEN=<token>
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_Vue" -D"sonar.projectName=UIs Vue" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
```

### Deploy to Azure Static Web App
- Create **staticwebapp.config.json** in the **./dist** folder
```json
{
    "navigationFallback": {
        "rewrite": "/index.html"
    }
}
```
- Install Azure Static Web Apps CLI
```bash
npm install -g @azure/static-web-apps-cli
```
- Deploy
```bash
swa deploy ./dist --app-name vue --env production
```
