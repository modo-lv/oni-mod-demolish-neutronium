@echo off

taskkill /f /im OxygenNotIncluded.exe

rem Taskkill exits with an erorrlevel 128 if the process isn't found,
rem but as long as ONI is closed, we don't care.
if errorlevel 128 exit /b 0