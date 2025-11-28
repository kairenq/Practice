@echo off
echo ==========================================
echo    СБОРКА ПРИЕМНАЯ КОМИССИЯ БППК
echo ==========================================
echo.

echo Проверка .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo ОШИБКА: .NET SDK не установлен!
    echo Скачайте и установите .NET 8.0 SDK с https://dotnet.microsoft.com/download
    pause
    exit /b 1
)

echo.
echo Восстановление пакетов NuGet...
dotnet restore

if errorlevel 1 (
    echo ОШИБКА при восстановлении пакетов!
    pause
    exit /b 1
)

echo.
echo Сборка проекта...
dotnet build --configuration Release

if errorlevel 1 (
    echo ОШИБКА при сборке проекта!
    pause
    exit /b 1
)

echo.
echo ==========================================
echo    СБОРКА ЗАВЕРШЕНА УСПЕШНО!
echo ==========================================
echo.
echo Запустите run.bat для запуска приложения
pause
