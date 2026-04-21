@echo off
setlocal

REM ==============================
REM Konfigurasi
set FRISTA_EXE=%~dp0frista.exe
set PROTOCOL_NAME=fcbpjs-frista
set HELPER_BAT=%~dp0frista-helper.bat
REM ==============================

echo ===========================================
echo       Frista All-in-One Launcher
echo ===========================================
echo.

REM ======= Cek frista.exe =======
if not exist "%FRISTA_EXE%" (
    echo ERROR: frista.exe tidak ditemukan di folder ini:
    echo "%~dp0"
    pause
    exit /b
)

echo frista.exe ditemukan.
echo.

REM ======= Buat helper BAT untuk protocol =======
(
echo @echo off
echo start "" "%FRISTA_EXE%"
echo exit
) > "%HELPER_BAT%"
echo Helper BAT dibuat di "%HELPER_BAT%"
echo.

REM ======= Daftarkan protocol custom =======
echo Mendaftarkan protocol %PROTOCOL_NAME%:// ...
REG ADD "HKCR\%PROTOCOL_NAME%" /ve /d "URL:Frista Protocol" /f >nul
REG ADD "HKCR\%PROTOCOL_NAME%" /v "URL Protocol" /d "" /f >nul
REG ADD "HKCR\%PROTOCOL_NAME%\shell\open\command" /ve /d "\"%HELPER_BAT%\" \"%%1\"" /f >nul
echo Protocol %PROTOCOL_NAME%:// berhasil didaftarkan!
echo.

REM ======= Jalankan frista.exe =======
echo Menjalankan frista.exe...
start "" "%FRISTA_EXE%"
echo Selesai.
echo.

REM ======= Tampilkan tombol HTML siap copy-paste =======
echo Tombol HTML siap copy-paste:
echo.
echo ^<a href="%PROTOCOL_NAME%://test" class="btn btn-success"^>
echo     Jalankan Frista
echo ^</a^>
echo.
pause
exit
