@echo off
chcp 65001 >nul
echo ==========================================
echo   ПРИЕМНАЯ КОМИССИЯ БППК
echo   Быстрый запуск
echo ==========================================
echo.
echo Выберите действие:
echo.
echo 1. Запустить базу данных (PostgreSQL)
echo 2. Собрать приложение
echo 3. Запустить приложение
echo 4. Создать EXE файл
echo 5. Полный запуск (БД + Сборка + Запуск)
echo 0. Выход
echo.
set /p choice="Введите номер: "

if "%choice%"=="1" goto start_db
if "%choice%"=="2" goto build
if "%choice%"=="3" goto run
if "%choice%"=="4" goto publish
if "%choice%"=="5" goto full_start
if "%choice%"=="0" exit
goto menu

:start_db
call start-db.bat
pause
exit

:build
call build.bat
pause
exit

:run
call run.bat
pause
exit

:publish
call publish.bat
pause
exit

:full_start
echo.
echo Шаг 1/3: Запуск базы данных...
call start-db.bat
timeout /t 5 /nobreak >nul
echo.
echo Шаг 2/3: Сборка приложения...
call build.bat
echo.
echo Шаг 3/3: Запуск приложения...
call run.bat
exit
