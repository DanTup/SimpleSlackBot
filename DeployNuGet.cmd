@echo off

pushd %~dp0

rem Clean + Build everything.
msbuild /nologo /v:q /t:Clean,Build /p:"Configuration=Release"

rem If build filed, bail.
if %errorlevel% neq 0 exit /b %errorlevel%

rem Build NuGet package.
pushd SimpleSlackBot
nuget pack -Prop Configuration=Release

rem If building NuGet filed, bail.
if %errorlevel% neq 0 exit /b %errorlevel%

nuget push *.nupkg
del *.nupkg
