@echo off
echo ==========================================
echo   СОЗДАНИЕ EXE ФАЙЛА
echo ==========================================
echo.

echo Создание standalone EXE файла...
echo Это может занять несколько минут...
echo.

dotnet publish --configuration Release --runtime win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true --output ./publish

if errorlevel 1 (
    echo ОШИБКА при создании EXE!
    pause
    exit /b 1
)

echo.
echo ==========================================
echo   EXE ФАЙЛ СОЗДАН УСПЕШНО!
echo ==========================================
echo.
echo Файл находится в папке: publish\AdmissionSystem.exe
echo.
echo ВНИМАНИЕ: Для работы приложения нужен PostgreSQL!
echo Настройте appsettings.json в папке publish
pause
