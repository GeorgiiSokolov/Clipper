
#define RevitYear 2025
#define MyAppName "Clipper for Revit "
#define MyAppVersion "0.03a"
#define MyAppPublisher "Georgii Sokolov"
#define MyAppURL "https://github.com/GeorgiiSokolov/Clipper"

; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)

;2023 "61FA20A2-1E14-4E5E-AA21-AAF3D860E29F"
;2024 "2AB30B3-2F25-4F6F-BB32-BBF4E970F4E0"
#define MyAppId "83BC40C4-3G36-5G7G-CC43-CCG5F081G5F1"

[Setup]
AppId={{{#MyAppId}}
AppName={#MyAppName}{#RevitYear}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}{#RevitYear}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=no
; Uncomment the following line to run in non administrative install mode (install for current user only).
PrivilegesRequired=lowest
;PrivilegesRequiredOverridesAllowed=dialog
OutputDir=..\Releases
OutputBaseFilename=clipper-{#MyAppVersion}-{#RevitYear}
SolidCompression=yes
WizardStyle=modern
UninstallFilesDir={userappdata}\Autodesk\Revit\Addins\{#RevitYear}\Clipper\Uninstall

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\src\bin\Release_{#RevitYear}\Clipper.dll"; DestDir: "{userappdata}\Autodesk\Revit\Addins\{#RevitYear}\Clipper"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
; Copy .addin file to Revit {#RevitYear} Addins folder
Source: "..\addin\Clipper.addin"; DestDir: "{userappdata}\Autodesk\Revit\Addins\{#RevitYear}"; Flags: ignoreversion

[Dirs]
Name: "{userappdata}\Autodesk\Revit\Addins\{#RevitYear}"; Flags: uninsneveruninstall