#!/bin/bash
# ========================================
# Git Flow Implementation for TaskFlow PIM
# ========================================

show_help() {
    echo ""
    echo "🌊 GIT FLOW - TaskFlow PIM"
    echo "========================"
    echo ""
    echo "Comandos disponibles:"
    echo ""
    echo "  ./gitflow.sh init                    - Inicializar Git Flow"
    echo "  ./gitflow.sh status                  - Ver estado de ramas"
    echo ""
    echo "  ./gitflow.sh feature start <name>   - Crear nueva feature"
    echo "  ./gitflow.sh feature finish <name>  - Finalizar feature"
    echo "  ./gitflow.sh feature list           - Listar features activas"
    echo ""
    echo "  ./gitflow.sh release start <version> - Crear nueva release"
    echo "  ./gitflow.sh release finish <version> - Finalizar release"
    echo ""
    echo "  ./gitflow.sh hotfix start <version>  - Crear hotfix"
    echo "  ./gitflow.sh hotfix finish <version> - Finalizar hotfix"
    echo ""
    echo "Estructura de ramas:"
    echo "  main     - Rama principal (producción)"
    echo "  develop  - Rama de desarrollo"
    echo "  feature/ - Ramas de características"
    echo "  release/ - Ramas de release"
    echo "  hotfix/  - Ramas de hotfix"
    echo ""
}

init_gitflow() {
    echo "🚀 Inicializando Git Flow..."
    git checkout main
    git pull origin main
    git checkout -b develop 2>/dev/null || git checkout develop
    git push -u origin develop 2>/dev/null
    echo "✅ Git Flow inicializado correctamente"
    echo "   - main: rama principal"
    echo "   - develop: rama de desarrollo"
}

show_status() {
    echo "📊 Estado de Git Flow:"
    echo ""
    echo "🌿 Ramas locales:"
    git branch
    echo ""
    echo "🌐 Ramas remotas:"
    git branch -r
    echo ""
    echo "📍 Rama actual:"
    git branch --show-current
}

feature_help() {
    echo ""
    echo "🔧 Git Flow - Features"
    echo "===================="
    echo ""
    echo "  ./gitflow.sh feature start <name>   - Crear nueva feature desde develop"
    echo "  ./gitflow.sh feature finish <name>  - Finalizar feature y merge a develop"
    echo "  ./gitflow.sh feature list           - Listar features activas"
    echo ""
}

feature_list() {
    echo "🔧 Features activas:"
    git branch | grep "feature/" || echo "  No hay features activas"
}

feature_start() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica el nombre de la feature"
        echo "   Uso: ./gitflow.sh feature start <nombre>"
        return 1
    fi
    
    echo "🔧 Creando feature: $1"
    git checkout develop
    git pull origin develop
    git checkout -b "feature/$1"
    git push -u origin "feature/$1"
    echo "✅ Feature 'feature/$1' creada y lista para desarrollo"
}

feature_finish() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica el nombre de la feature"
        echo "   Uso: ./gitflow.sh feature finish <nombre>"
        return 1
    fi
    
    echo "🔧 Finalizando feature: $1"
    git checkout develop
    git pull origin develop
    git merge --no-ff "feature/$1"
    git push origin develop
    git branch -d "feature/$1"
    git push origin --delete "feature/$1"
    echo "✅ Feature 'feature/$1' finalizada y mergeada a develop"
}

release_help() {
    echo ""
    echo "🚀 Git Flow - Releases"
    echo "===================="
    echo ""
    echo "  ./gitflow.sh release start <version>  - Crear nueva release desde develop"
    echo "  ./gitflow.sh release finish <version> - Finalizar release y merge a main"
    echo ""
}

release_start() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica la versión del release"
        echo "   Uso: ./gitflow.sh release start <version>"
        echo "   Ejemplo: ./gitflow.sh release start v1.2.0"
        return 1
    fi
    
    echo "🚀 Creando release: $1"
    git checkout develop
    git pull origin develop
    git checkout -b "release/$1"
    git push -u origin "release/$1"
    echo "✅ Release 'release/$1' creado y listo para preparación"
}

release_finish() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica la versión del release"
        echo "   Uso: ./gitflow.sh release finish <version>"
        return 1
    fi
    
    echo "🚀 Finalizando release: $1"
    git checkout main
    git pull origin main
    git merge --no-ff "release/$1"
    git tag -a "$1" -m "Release $1"
    git push origin main
    git push origin "$1"
    git checkout develop
    git merge --no-ff "release/$1"
    git push origin develop
    git branch -d "release/$1"
    git push origin --delete "release/$1"
    echo "✅ Release '$1' finalizado y desplegado a main"
}

hotfix_help() {
    echo ""
    echo "🔥 Git Flow - Hotfixes"
    echo "===================="
    echo ""
    echo "  ./gitflow.sh hotfix start <version>   - Crear hotfix desde main"
    echo "  ./gitflow.sh hotfix finish <version>  - Finalizar hotfix y merge a main/develop"
    echo ""
}

hotfix_start() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica la versión del hotfix"
        echo "   Uso: ./gitflow.sh hotfix start <version>"
        echo "   Ejemplo: ./gitflow.sh hotfix start v1.1.1"
        return 1
    fi
    
    echo "🔥 Creando hotfix: $1"
    git checkout main
    git pull origin main
    git checkout -b "hotfix/$1"
    git push -u origin "hotfix/$1"
    echo "✅ Hotfix 'hotfix/$1' creado y listo para corrección"
}

hotfix_finish() {
    if [ -z "$1" ]; then
        echo "❌ Error: Especifica la versión del hotfix"
        echo "   Uso: ./gitflow.sh hotfix finish <version>"
        return 1
    fi
    
    echo "🔥 Finalizando hotfix: $1"
    git checkout main
    git pull origin main
    git merge --no-ff "hotfix/$1"
    git tag -a "$1" -m "Hotfix $1"
    git push origin main
    git push origin "$1"
    git checkout develop
    git merge --no-ff "hotfix/$1"
    git push origin develop
    git branch -d "hotfix/$1"
    git push origin --delete "hotfix/$1"
    echo "✅ Hotfix '$1' finalizado y aplicado a main y develop"
}

# Main script logic
case "$1" in
    "init")
        init_gitflow
        ;;
    "status")
        show_status
        ;;
    "feature")
        case "$2" in
            "start")
                feature_start "$3"
                ;;
            "finish")
                feature_finish "$3"
                ;;
            "list")
                feature_list
                ;;
            *)
                feature_help
                ;;
        esac
        ;;
    "release")
        case "$2" in
            "start")
                release_start "$3"
                ;;
            "finish")
                release_finish "$3"
                ;;
            *)
                release_help
                ;;
        esac
        ;;
    "hotfix")
        case "$2" in
            "start")
                hotfix_start "$3"
                ;;
            "finish")
                hotfix_finish "$3"
                ;;
            *)
                hotfix_help
                ;;
        esac
        ;;
    *)
        show_help
        ;;
esac
