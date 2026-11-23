!include "MUI2.nsh"

!ifndef APP_SOURCE
  !error "APP_SOURCE is required"
!endif
!ifndef APP_VERSION
  !error "APP_VERSION is required"
!endif
!ifndef APP_ICON
  !error "APP_ICON is required"
!endif

Name "MAGU"
OutFile "MAGU-Windows-x64-setup.exe"
InstallDir "$LOCALAPPDATA\Programs\MAGU"
InstallDirRegKey HKCU "Software\MAGU" "InstallDir"
RequestExecutionLevel user
Unicode true
SetCompressor /SOLID lzma

VIProductVersion "${APP_VERSION}.0"
VIAddVersionKey /LANG=1033 "FileVersion" "${APP_VERSION}.0"
VIAddVersionKey /LANG=1033 "ProductName" "MAGU"
VIAddVersionKey /LANG=1033 "FileDescription" "MAGU Windows desktop application"
VIAddVersionKey /LANG=1033 "CompanyName" "MAGU"
VIAddVersionKey /LANG=1033 "LegalCopyright" "MIT License"

!define MUI_ABORTWARNING
!define MUI_ICON "${APP_ICON}"
!define MUI_UNICON "${APP_ICON}"
!define MUI_WELCOMEPAGE_TITLE "MAGU kurulumu"
!define MUI_WELCOMEPAGE_TEXT "MAGU'yu bilgisayarınıza kurun. Kullanıcı verileriniz ve ayarlarınız korunur."
!define MUI_DIRECTORYPAGE_TEXT_TOP "MAGU'nun kurulacağı klasörü seçin."
!define MUI_DIRECTORYPAGE_TEXT_DESTINATION "Kurulum klasörü"
!define MUI_FINISHPAGE_RUN "$INSTDIR\Nashi.exe"
!define MUI_FINISHPAGE_RUN_TEXT "MAGU'yu başlat"
!define MUI_FINISHPAGE_LINK "MAGU"
!define MUI_FINISHPAGE_LINK_LOCATION "https://github.com/metinakinguloglu-cmd/MAGU"

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH
!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES

!insertmacro MUI_LANGUAGE "Turkish"
!insertmacro MUI_LANGUAGE "English"

Section "MAGU" MainSection
  SectionIn RO
  SetOutPath "$INSTDIR"
  File /r "${APP_SOURCE}\*"
  CreateDirectory "$SMPROGRAMS\MAGU"
  CreateShortCut "$SMPROGRAMS\MAGU\MAGU.lnk" "$INSTDIR\Nashi.exe"
  WriteRegStr HKCU "Software\MAGU" "InstallDir" "$INSTDIR"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU" "DisplayName" "MAGU"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU" "DisplayVersion" "${APP_VERSION}"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU" "Publisher" "MAGU"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU" "InstallLocation" "$INSTDIR"
  WriteRegStr HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU" "UninstallString" '"$INSTDIR\Uninstall.exe"'
  WriteUninstaller "$INSTDIR\Uninstall.exe"
SectionEnd

Section /o "Masaüstü kısayolu" DesktopShortcut
  CreateShortCut "$DESKTOP\MAGU.lnk" "$INSTDIR\Nashi.exe"
SectionEnd

Section "Uninstall"
  Delete "$SMPROGRAMS\MAGU\MAGU.lnk"
  RMDir "$SMPROGRAMS\MAGU"
  Delete "$DESKTOP\MAGU.lnk"
  DeleteRegKey HKCU "Software\Microsoft\Windows\CurrentVersion\Uninstall\MAGU"
  DeleteRegKey HKCU "Software\MAGU"
  RMDir /r "$INSTDIR"
SectionEnd
