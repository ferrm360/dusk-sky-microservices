#!/bin/bash

chmod +x Start-Containers-Tony.sh

OS=$(uname -s)

if [[ "$OS" == "Linux" ]]; then
    echo "Iniciando contenedores en Linux..."

    docker compose -f docker-compose.infra.yml up --build -d

    docker compose -f CommentService/CommentService.Api/docker-compose.yml up --build -d

elif [[ "$OS" == "CYGWIN"* || "$OS" == "MINGW"* ]]; then
    echo "Iniciando contenedores en Windows..."

    powershell.exe -Command "docker compose -f docker-compose.infra.yml up --build -d"
    powershell.exe -Command "docker compose -f CommentService/CommentService.Api/docker-compose.yml up --build -d"

else
    echo "Sistema operativo no soportado"
    exit 1
fi
