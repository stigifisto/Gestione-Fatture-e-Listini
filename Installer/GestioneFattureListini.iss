; Script Inno Setup per "Gestione Fatture e Listini".
; Compilazione: apri questo file con Inno Setup Compiler (ISCC.exe) oppure, da riga di comando:
;   "C:\Users\<utente>\AppData\Local\Programs\Inno Setup 6\ISCC.exe" GestioneFattureListini.iss
; Prerequisito: la solution va compilata in configurazione Release prima di lanciare il setup
; (Build > Rebuild in Release, oppure msbuild /p:Configuration=Release).

#define MyAppName "Gestione Fatture e Listini"
#define MyAppVersion "1.0"
#define MyAppExeName "Gestione Fatture e Listini.exe"
#define MyBuildDir "..\Gestione Fatture e Listini\bin\Release"

[Setup]
AppId={{8F1B6C2E-2D3A-4E3B-9E6C-1B7A5B6C0F31}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
DisableProgramGroupPage=yes
OutputDir=Output
OutputBaseFilename=GestioneFattureListini_Setup
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "italian"; MessagesFile: "compiler:Languages\Italian.isl"

[Tasks]
Name: "desktopicon"; Description: "Crea un'icona sul desktop"; GroupDescription: "Icone aggiuntive:"
Name: "schedulazione"; Description: "Configura l'importazione notturna pianificata (listini AS400/Infinity e fatture AS400, ogni giorno alle 03:00)"; GroupDescription: "Attività pianificate:"; Flags: unchecked

[Files]
Source: "{#MyBuildDir}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyBuildDir}\{#MyAppExeName}.config"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyBuildDir}\*.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyBuildDir}\Estrattore.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#MyBuildDir}\estrai_data_listino.exe"; DestDir: "{app}"; Flags: ignoreversion

[Dirs]
Name: "C:\appoggio\MD FATTURE\ZIP"
Name: "C:\appoggio\MD FATTURE\P7M"
Name: "C:\appoggio\MD FATTURE\XML"
Name: "C:\appoggio\MD FATTURE\BACKUPXML"
Name: "C:\appoggio\MD FATTURE\XMLDOPPI"

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Disinstalla {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{sys}\schtasks.exe"; Parameters: "/Create /TN ""Gestione Fatture e Listini - Import Notturno"" /TR ""\""{app}\{#MyAppExeName}\"" /listini"" /SC DAILY /ST 03:00 /RL HIGHEST /F"; Tasks: schedulazione; Flags: runhidden
Filename: "{app}\{#MyAppExeName}"; Description: "Avvia {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallRun]
Filename: "{cmd}"; Parameters: "/C schtasks.exe /Delete /TN ""Gestione Fatture e Listini - Import Notturno"" /F >nul 2>&1"; Flags: runhidden; RunOnceId: "RimuoviTaskListini"

[Code]
function IsDotNet472Installed(): Boolean;
var
  release: Cardinal;
begin
  Result := RegQueryDWordValue(HKLM, 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full', 'Release', release) and (release >= 461808);
end;

function IsIBMClientAccessInstalled(): Boolean;
begin
  Result := FileExists(ExpandConstant('{pf32}\IBM\Client Access\IBM.Data.DB2.iSeries.dll'))
    or FileExists(ExpandConstant('{pf}\IBM\Client Access\IBM.Data.DB2.iSeries.dll'));
end;

function InitializeSetup(): Boolean;
begin
  Result := True;

  if not IsDotNet472Installed() then
    MsgBox('Non risulta installato .NET Framework 4.7.2 (o superiore) su questo PC.' + #13#10 + #13#10 +
           'Il programma lo richiede: se necessario, scaricalo da https://dotnet.microsoft.com/download/dotnet-framework prima di avviare "' + '{#MyAppName}' + '".' + #13#10 + #13#10 +
           'Il setup continuera comunque.',
           mbInformation, MB_OK);

  if not IsIBMClientAccessInstalled() then
    MsgBox('Non risulta installato il driver "IBM iSeries Access Client", necessario per il collegamento con AS400.' + #13#10 + #13#10 +
           'Il setup non lo include (e un prodotto IBM a licenza separata): installalo a parte prima di usare le funzioni di importazione da AS400.' + #13#10 + #13#10 +
           'Il setup continuera comunque.',
           mbInformation, MB_OK);
end;
