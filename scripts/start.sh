#!/usr/bin/env bash
# Levanta PostgreSQL (Docker), la API y Blazor. Ctrl+C para parar API y Web.
set -euo pipefail
cd "$(dirname "$0")/.."

docker compose up -d db

dotnet run --project src/Ecommerce.Api &
API_PID=$!
trap 'kill $API_PID 2>/dev/null || true' EXIT

dotnet run --project src/Ecommerce.Web
