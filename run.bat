@echo off
echo ==========================================
echo   ЗАПУСК ПРИЕМНАЯ КОМИССИЯ БППК
echo ==========================================
echo.

echo Проверка PostgreSQL...
echo ВАЖНО: Убедитесь, что PostgreSQL запущен!
echo Вы можете запустить его через: docker-compose up -d
echo.

echo Запуск приложения...
dotnet run

if errorlevel 1 (
    echo.
    echo ОШИБКА при запуске!
    echo.
    echo Возможные причины:
    echo 1. PostgreSQL не запущен (запустите: docker-compose up -d)
    echo 2. Проект не собран (запустите build.bat)
    echo 3. Неверные настройки подключения в appsettings.json
    pause
    exit /b 1
)
