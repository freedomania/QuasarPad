; QuasarPad — Inno Setup script (English UI)
; 1. Put QuasarPad.ico in src\QuasarPad\Assets\ (optional but recommended)
; 2. Publish Small build to ..\publish\Small
; 3. Open this file in Inno Setup → Build → Compile

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
; Icon for the setup.exe and shortcuts (file must exist before Compile)
SetupIconFile=..\src\QuasarPad\Assets\QuasarPad.ico
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
Source: "..\publish\Small\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch QuasarPad"; Flags: nowait postinstall skipifsilent
