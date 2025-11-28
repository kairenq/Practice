@echo off
echo ==========================================
echo   ЗАПУСК БАЗЫ ДАННЫХ POSTGRESQL
echo ==========================================
echo.

echo Проверка Docker...
docker --version >nul 2>&1
if errorlevel 1 (
    echo ОШИБКА: Docker не установлен!
    echo Скачайте и установите Docker Desktop с https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo.
echo Запуск PostgreSQL через Docker Compose...
docker-compose up -d

if errorlevel 1 (
    echo ОШИБКА при запуске PostgreSQL!
    pause
    exit /b 1
)

echo.
echo ==========================================
echo   PostgreSQL ЗАПУЩЕН УСПЕШНО!
echo ==========================================
echo.
echo База данных доступна по адресу: localhost:5432
echo База: admission_bppk
echo Пользователь: postgres
echo Пароль: postgres
echo.
echo Для остановки используйте: docker-compose down
pause
