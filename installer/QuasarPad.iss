; QuasarPad — Inno Setup script
; 1. Publish Small build to ..\publish\Small
; 2. Open this file in Inno Setup and Compile

#define MyAppName "QuasarPad"
#define MyAppVersion "1.5.0"
#define MyAppPublisher "QuasarPad"
#define MyAppURL "https://github.com/freedomania/QuasarPad"
#define MyAppExeName "QuasarPad.exe"

[Setup]
AppId={{A7B3C9D1-4E5F-6789-ABCD-EF0123456789}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName=C:\QuasarPad
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=..\publish\Installer
OutputBaseFilename=QuasarPad-Setup-{#MyAppVersion}
SetupIconFile=
Compression=lzma
SolidCompression=yes
WizardStyle=modern
PrivilegesRequired=lowest
ArchitecturesInstallIn64BitMode=x64compatible

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional icons:"; Flags: unchecked

[Files]
; Publish Small build first: publish\Small\*
Source: "..\publish\Small\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch QuasarPad"; Flags: nowait postinstall skipifsilent
