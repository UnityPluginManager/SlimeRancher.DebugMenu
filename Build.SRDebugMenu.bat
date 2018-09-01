@echo off

set BaseDir=%cd%

cd %BaseDir%
dotnet build SrDebug.csproj -c Release

echo Press any key to exit . . . 
pause > nul
