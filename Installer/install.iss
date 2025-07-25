; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Clipper"
#define MyAppVersion "0.02 alpha"
#define MyAppPublisher "Georgii Sokolov"
#define MyAppURL "https://github.com/GeorgiiSokolov/Clipper"
;#define MyAppExeName "MyProg-x64.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{61FA20A2-1E14-4E5E-AA21-AAF3D860E29F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=no
; Uncomment the following line to run in non administrative install mode (install for current user only).
PrivilegesRequired=lowest
;PrivilegesRequiredOverridesAllowed=dialog
OutputDir=C:\_GSV_WORK\Prog\Clipper\Releases
OutputBaseFilename=clipper-0.02a
SolidCompression=yes
WizardStyle=modern
UninstallFilesDir={localappdata}\Autodesk\Revit\Addins\2023\Clipper\Uninstall

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Files]
Source: "..\src\bin\Release\net48\Clipper.dll"; DestDir: "{localappdata}\Autodesk\Revit\Addins\2023\Clipper"; Flags: ignoreversion
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
; Copy .addin file to Revit 2023 Addins folder
Source: "..\addin\Clipper.addin"; DestDir: "{localappdata}\Autodesk\Revit\Addins\2023"; Flags: ignoreversion

[Dirs]
Name: "{localappdata}\Autodesk\Revit\Addins\2023"; Flags: uninsneveruninstall