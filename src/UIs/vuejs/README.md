# vuejs

## Project setup
```
npm install
```

### Compiles and hot-reloads for development
```
npm run serve
```

### Compiles and minifies for production
```
npm run build
```

### Lints and fixes files
```
npm run lint
```

### Customize configuration
See [Configuration Reference](https://cli.vuejs.org/config/).

### SonarQube

```
echo %SONAR_TOKEN%
setx SONAR_TOKEN <token>
set SONAR_TOKEN=<token>
sonar-scanner.bat -D"sonar.organization=phongnguyend" -D"sonar.projectKey=UIs_Vue" -D"sonar.projectName=UIs Vue" -D"sonar.projectVersion=1.0.0.0" -D"sonar.sources=." -D"sonar.host.url=https://sonarcloud.io"
```
