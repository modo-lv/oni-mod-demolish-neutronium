cd /d %~dp0
robocopy bin\Release c:\users\martin\Documents\Klei\OxygenNotIncluded\mods\Local\DemolishNeutronium *.yaml *.md *.png DemolishNeutronium.dll /mir ^
if ERRORLEVEL goto FAIL

c:\games\steam\steam.exe -applaunch 457140
goto END

:FAIL
pause
:END