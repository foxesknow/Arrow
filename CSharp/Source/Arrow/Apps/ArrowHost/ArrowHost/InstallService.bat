@echo off
rem Installs the ArrowHost service with the unique ServiceName and an appropriate DisplayName
rem NOTE: The spaces in the SC command after the = sign ARE significant!

if "%1"=="" goto :usage
if "%~2"=="" goto :usage

set ServiceName=%1
set DisplayName=%~2

set THISDIR=%~dp0
sc create %ServiceName% binpath= "%THISDIR%ArrowHost.exe" displayname= "%DisplayName%" depend= Tcpip start= auto
echo Service installed
goto :exit

:usage
echo.
echo usage:
echo InstallService ServiceName DisplayName
echo.

:exit
