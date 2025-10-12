@echo off
cd /d "%~dp0"

echo === Building Releases ===

rmdir /s /q src\obj
rmdir /s /q src\bin

dotnet build src -c Release_2023 || exit /b
dotnet build src -c Release_2024 || exit /b
dotnet build src -c Release_2025 || exit /b

echo === Building done ===
echo === Building installers ===

"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /Qp "Installer\install_2023.iss" || exit /b
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /Qp "Installer\install_2024.iss" || exit /b
"C:\Program Files (x86)\Inno Setup 6\ISCC.exe" /Qp "Installer\install_2025.iss" || exit /b

echo === All releases built successfully ===