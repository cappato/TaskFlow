@echo off
REM ========================================
REM Git Flow Implementation for TaskFlow PIM
REM ========================================

if "%1"=="" goto :help
if "%1"=="help" goto :help
if "%1"=="init" goto :init
if "%1"=="feature" goto :feature
if "%1"=="release" goto :release
if "%1"=="hotfix" goto :hotfix
if "%1"=="status" goto :status

goto :help

:help
echo.
echo 🌊 GIT FLOW - TaskFlow PIM
echo ========================
echo.
echo Comandos disponibles:
echo.
echo   gitflow init                    - Inicializar Git Flow
echo   gitflow status                  - Ver estado de ramas
echo.
echo   gitflow feature start ^<name^>   - Crear nueva feature
echo   gitflow feature finish ^<name^>  - Finalizar feature
echo   gitflow feature list            - Listar features activas
echo.
echo   gitflow release start ^<version^> - Crear nueva release
echo   gitflow release finish ^<version^> - Finalizar release
echo.
echo   gitflow hotfix start ^<version^>  - Crear hotfix
echo   gitflow hotfix finish ^<version^> - Finalizar hotfix
echo.
echo Estructura de ramas:
echo   main     - Rama principal (producción)
echo   develop  - Rama de desarrollo
echo   feature/ - Ramas de características
echo   release/ - Ramas de release
echo   hotfix/  - Ramas de hotfix
echo.
goto :end

:init
echo 🚀 Inicializando Git Flow...
git checkout main
git pull origin main
git checkout -b develop 2>nul || git checkout develop
git push -u origin develop 2>nul
echo ✅ Git Flow inicializado correctamente
echo    - main: rama principal
echo    - develop: rama de desarrollo
goto :end

:status
echo 📊 Estado de Git Flow:
echo.
echo 🌿 Ramas locales:
git branch
echo.
echo 🌐 Ramas remotas:
git branch -r
echo.
echo 📍 Rama actual:
git branch --show-current
goto :end

:feature
if "%2"=="" goto :feature_help
if "%2"=="list" goto :feature_list
if "%2"=="start" goto :feature_start
if "%2"=="finish" goto :feature_finish
goto :feature_help

:feature_help
echo.
echo 🔧 Git Flow - Features
echo ====================
echo.
echo   gitflow feature start ^<name^>   - Crear nueva feature desde develop
echo   gitflow feature finish ^<name^>  - Finalizar feature y merge a develop
echo   gitflow feature list            - Listar features activas
echo.
goto :end

:feature_list
echo 🔧 Features activas:
git branch | findstr "feature/"
if errorlevel 1 echo   No hay features activas
goto :end

:feature_start
if "%3"=="" (
    echo ❌ Error: Especifica el nombre de la feature
    echo    Uso: gitflow feature start ^<nombre^>
    goto :end
)
echo 🔧 Creando feature: %3
git checkout develop
git pull origin develop
git checkout -b feature/%3
git push -u origin feature/%3
echo ✅ Feature 'feature/%3' creada y lista para desarrollo
goto :end

:feature_finish
if "%3"=="" (
    echo ❌ Error: Especifica el nombre de la feature
    echo    Uso: gitflow feature finish ^<nombre^>
    goto :end
)
echo 🔧 Finalizando feature: %3
git checkout develop
git pull origin develop
git merge --no-ff feature/%3
git push origin develop
git branch -d feature/%3
git push origin --delete feature/%3
echo ✅ Feature 'feature/%3' finalizada y mergeada a develop
goto :end

:release
if "%2"=="" goto :release_help
if "%2"=="start" goto :release_start
if "%2"=="finish" goto :release_finish
goto :release_help

:release_help
echo.
echo 🚀 Git Flow - Releases
echo ====================
echo.
echo   gitflow release start ^<version^>  - Crear nueva release desde develop
echo   gitflow release finish ^<version^> - Finalizar release y merge a main
echo.
goto :end

:release_start
if "%3"=="" (
    echo ❌ Error: Especifica la versión del release
    echo    Uso: gitflow release start ^<version^>
    echo    Ejemplo: gitflow release start v1.2.0
    goto :end
)
echo 🚀 Creando release: %3
git checkout develop
git pull origin develop
git checkout -b release/%3
git push -u origin release/%3
echo ✅ Release 'release/%3' creado y listo para preparación
goto :end

:release_finish
if "%3"=="" (
    echo ❌ Error: Especifica la versión del release
    echo    Uso: gitflow release finish ^<version^>
    goto :end
)
echo 🚀 Finalizando release: %3
git checkout main
git pull origin main
git merge --no-ff release/%3
git tag -a %3 -m "Release %3"
git push origin main
git push origin %3
git checkout develop
git merge --no-ff release/%3
git push origin develop
git branch -d release/%3
git push origin --delete release/%3
echo ✅ Release '%3' finalizado y desplegado a main
goto :end

:hotfix
if "%2"=="" goto :hotfix_help
if "%2"=="start" goto :hotfix_start
if "%2"=="finish" goto :hotfix_finish
goto :hotfix_help

:hotfix_help
echo.
echo 🔥 Git Flow - Hotfixes
echo ====================
echo.
echo   gitflow hotfix start ^<version^>   - Crear hotfix desde main
echo   gitflow hotfix finish ^<version^>  - Finalizar hotfix y merge a main/develop
echo.
goto :end

:hotfix_start
if "%3"=="" (
    echo ❌ Error: Especifica la versión del hotfix
    echo    Uso: gitflow hotfix start ^<version^>
    echo    Ejemplo: gitflow hotfix start v1.1.1
    goto :end
)
echo 🔥 Creando hotfix: %3
git checkout main
git pull origin main
git checkout -b hotfix/%3
git push -u origin hotfix/%3
echo ✅ Hotfix 'hotfix/%3' creado y listo para corrección
goto :end

:hotfix_finish
if "%3"=="" (
    echo ❌ Error: Especifica la versión del hotfix
    echo    Uso: gitflow hotfix finish ^<version^>
    goto :end
)
echo 🔥 Finalizando hotfix: %3
git checkout main
git pull origin main
git merge --no-ff hotfix/%3
git tag -a %3 -m "Hotfix %3"
git push origin main
git push origin %3
git checkout develop
git merge --no-ff hotfix/%3
git push origin develop
git branch -d hotfix/%3
git push origin --delete hotfix/%3
echo ✅ Hotfix '%3' finalizado y aplicado a main y develop
goto :end

:end
