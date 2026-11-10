param(
    [ValidateSet('All', 'Flutter', 'Kotlin', 'ReactNative')]
    [string]$App = 'All',
    [ValidateSet('All', 'Home', 'Products', 'Users', 'Roles', 'Configuration', 'Navigation')]
    [string]$Feature = 'All',
    [switch]$IncludeCrud,
    [string]$Serial,
    [string]$TestMethod
)
$ErrorActionPreference = 'Stop'
if ($TestMethod -and $Feature -eq 'All') { throw 'Specify -Feature when selecting a -TestMethod.' }
$mobileRoot = Split-Path $PSScriptRoot -Parent
$sdk = if ($env:ANDROID_HOME) { $env:ANDROID_HOME } else { Join-Path $env:LOCALAPPDATA 'Android/Sdk' }
$adb = Join-Path $sdk 'platform-tools/adb.exe'
if (!(Test-Path $adb)) { throw 'Set ANDROID_HOME to your Android SDK.' }
if (!$env:JAVA_HOME) {
    $studioJdk = 'C:/Program Files/Android/Android Studio/jbr'
    if (Test-Path $studioJdk) { $env:JAVA_HOME = $studioJdk }
}
$devices = @(& $adb devices | Select-String '^([^\s]+)\s+device$' | ForEach-Object { $_.Matches[0].Groups[1].Value })
if ($devices.Count -ne 1) { throw 'Connect exactly one authorized Android emulator/device for this test run.' }
if ($Serial -and $Serial -ne $devices[0]) { throw "Requested device $Serial is not the connected device." }
$Serial = $devices[0]
$apps = @{ Flutter='com.classifiedads.mobile.classified_ads_mobile'; Kotlin='com.classifiedads.mobile.kotlin'; ReactNative='com.classifiedads.mobile.reactnative' }
$selected = if ($App -eq 'All') { @('Flutter', 'Kotlin', 'ReactNative') } else { @($App) }
foreach ($name in $selected) {
    $installed = & $adb -s $Serial shell pm path $apps[$name]
    if ($LASTEXITCODE -ne 0 -or !$installed) { throw "Install the $name app before running UI tests." }
}
if ($selected -contains 'ReactNative') {
    try { $status = Invoke-WebRequest 'http://localhost:8081/status' -UseBasicParsing -TimeoutSec 5 }
    catch { throw 'Start React Native Metro first: cd src/Mobiles/ReactNative; npm start -- --localhost --port 8081' }
    $statusText = if ($status.Content -is [byte[]]) { [System.Text.Encoding]::UTF8.GetString($status.Content) } else { [string]$status.Content }
    if ($statusText -notmatch 'packager-status:running') { throw 'Port 8081 is not a running Metro server.' }
}
foreach ($port in @(44312, 44367, 8081)) {
    & $adb -s $Serial reverse "tcp:$port" "tcp:$port"
    if ($LASTEXITCODE -ne 0) { throw "Could not forward port $port." }
}
$run = Get-Date -Format 'yyyyMMdd-HHmmss'
$output = Join-Path $PSScriptRoot "artifacts/$run-$App-$Feature"
New-Item -ItemType Directory -Path $output -Force | Out-Null
$classes = $selected | ForEach-Object {
    $class = "com.classifiedads.mobile.uitests.${_}UiTest"
    if ($Feature -ne 'All') { $class += '$' + $Feature }
    if ($TestMethod) { "$class#$TestMethod" } else { $class }
}
$arguments = @('-p', (Join-Path $PSScriptRoot 'android'), 'connectedDebugAndroidTest', '--max-workers=2',
    "-Pandroid.testInstrumentationRunnerArguments.class=$($classes -join ',')",
    "-Pandroid.testInstrumentationRunnerArguments.runId=$run",
    "-Pandroid.testInstrumentationRunnerArguments.includeCrud=$($IncludeCrud.IsPresent.ToString().ToLowerInvariant())")
$env:ANDROID_HOME = $sdk
$exitCode = 1
$previousErrorPreference = $ErrorActionPreference
$testMutex = [System.Threading.Mutex]::new($false, 'Local\ClassifiedAdsMobileUiTests')
try {
    if (!$testMutex.WaitOne(0)) {
        $testMutex.Dispose()
        throw 'Another mobile UI test run is using the emulator. Wait for it to finish.'
    }
} catch [System.Threading.AbandonedMutexException] {
    # A terminated runner releases ownership to this process.
}
try {
    # Windows PowerShell turns native stderr into error records, including Gradle warnings.
    $ErrorActionPreference = 'Continue'
    & (Join-Path $mobileRoot 'Kotlin/gradlew.bat') @arguments 2>&1 | Tee-Object (Join-Path $output 'gradle.log')
    $exitCode = $LASTEXITCODE
} finally {
    & $adb -s $Serial pull "/data/local/tmp/classifiedads-ui-results/$run" (Join-Path $output 'screenshots') 2>$null | Out-Null
    foreach ($path in @('outputs/androidTest-results/connected', 'reports/androidTests/connected')) {
        $source = Join-Path $PSScriptRoot "android/app/build/$path"
        if (Test-Path $source) { Copy-Item $source (Join-Path $output ($path.Split('/')[0])) -Recurse }
    }
    Write-Host "UI test artifacts: $output"
    $ErrorActionPreference = $previousErrorPreference
    $testMutex.ReleaseMutex()
    $testMutex.Dispose()
}
exit $exitCode
