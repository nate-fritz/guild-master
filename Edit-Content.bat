@echo off
title GuildMaster Content Editor
echo Starting GuildMaster Content Editor (close this window to stop it)...
dotnet run --project "%~dp0tools\ContentEditor"
pause
