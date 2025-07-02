@echo off

cd /d "%~dp0"

if "%~1"=="" (
    goto USAGE
)
set oni=c:\users\martin\Documents\Klei\OxygenNotIncluded\mods
set project=%~1

if /i "%~2"=="-release" (
    set source=%project%\bin\Release
    set target=%oni%\Local
) else (
    set source=%project%\bin\Debug
    set target=%oni%\Dev
)

robocopy "%source%" "%target%\%project%" "%project%.dll" *.yaml preview.png *.md /mir /r:0

rem Robocopy exits with errorlevel 1 when any files are copied, but it should count as a successful run
if ERRORLEVEL 1 exit /b 0

goto :EOF

:USAGE
echo Usage: deploy.bat ^<ProjectFolder^> [-release]
echo:
echo Without "-release" deployment will use "Debug" as the sourcve and "Dev" ONI mod folder as destination.
echo:
pause
exit /b 1