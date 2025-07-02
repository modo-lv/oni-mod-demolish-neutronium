@echo off

cd /d "%~dp0"

call deploy.bat %* && start c:\games\steam\steam.exe -applaunch 457140