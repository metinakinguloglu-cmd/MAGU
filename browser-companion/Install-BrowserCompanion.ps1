param(
  [Parameter(Mandatory=$true)][string]$ChromeExtensionId,
  [Parameter(Mandatory=$true)][string]$EdgeExtensionId,
  [string]$InstallRoot = "$env:LOCALAPPDATA\Programs\MAGU"
)
$ErrorActionPreference='Stop'
$hostExe=Join-Path $InstallRoot 'MAGU.BrowserHost.exe'
if(!(Test-Path $hostExe)){throw "MAGU.BrowserHost.exe bulunamadı: $hostExe"}
$manifest=Get-Content (Join-Path $PSScriptRoot 'native-host\com.magu.companion.json.template') -Raw
$manifest=$manifest.Replace('__MAGU_BROWSER_HOST_PATH__',$hostExe.Replace('\','\\')).Replace('__CHROME_EXTENSION_ID__',$ChromeExtensionId).Replace('__EDGE_EXTENSION_ID__',$EdgeExtensionId)
$manifestDir=Join-Path $env:LOCALAPPDATA 'MAGU\BrowserCompanion'
New-Item -ItemType Directory -Force -Path $manifestDir | Out-Null
$manifestPath=Join-Path $manifestDir 'com.magu.companion.json'
Set-Content -LiteralPath $manifestPath -Value $manifest -Encoding UTF8
$chromeKey='HKCU:\Software\Google\Chrome\NativeMessagingHosts\com.magu.companion'
$edgeKey='HKCU:\Software\Microsoft\Edge\NativeMessagingHosts\com.magu.companion'
New-Item -Path $chromeKey -Force | Out-Null;Set-ItemProperty -Path $chromeKey -Name '(default)' -Value $manifestPath
New-Item -Path $edgeKey -Force | Out-Null;Set-ItemProperty -Path $edgeKey -Name '(default)' -Value $manifestPath
Write-Host "Native Messaging hazırlandı: $manifestPath"
