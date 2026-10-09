# Levanta PostgreSQL (Docker), la API y Blazor. Cierra la ventana de la API al terminar.
Set-Location (Join-Path $PSScriptRoot "..")

docker compose up -d db

$api = Start-Process dotnet -ArgumentList "run --project src/Ecommerce.Api" -PassThru
try { dotnet run --project src/Ecommerce.Web }
finally { if (-not $api.HasExited) { Stop-Process -Id $api.Id } }
