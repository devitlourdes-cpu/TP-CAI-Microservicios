#!/usr/bin/env bash
#
# Levanta todos los microservicios de src/ y espera a que respondan.
# Ctrl+C corta todo.
#
#   ./scripts/run-all.sh
#
set -uo pipefail

RAIZ="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$RAIZ"

PIDS=()

# Al salir (Ctrl+C incluido) se bajan todos los servicios.
detener() {
  echo ""
  echo "Deteniendo microservicios..."
  for pid in "${PIDS[@]}"; do
    kill "$pid" 2>/dev/null
  done
  exit 0
}
trap detener INT TERM

echo "Compilando la solucion..."
if ! dotnet build ECommerce.sln -v quiet --nologo; then
  echo "ERROR: la solucion no compila. Corregir antes de la demo." >&2
  exit 1
fi

echo ""
for proyecto in src/*/*.csproj; do
  [ -e "$proyecto" ] || continue

  nombre=$(basename "$(dirname "$proyecto")")
  puerto=$(grep -o 'http://localhost:[0-9]*' "$(dirname "$proyecto")/Properties/launchSettings.json" | head -1 | grep -o '[0-9]*$')

  echo "Levantando $nombre en el puerto $puerto..."
  mkdir -p "$(dirname "$proyecto")/logs"
  ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_URLS="http://localhost:$puerto" \
    dotnet run --project "$proyecto" --no-build --no-launch-profile \
    > "$(dirname "$proyecto")/logs/consola.txt" 2>&1 &
  PIDS+=($!)

  # Se espera al health check para no mostrar la URL antes de que el servicio responda.
  for _ in $(seq 1 60); do
    if curl -s -o /dev/null "http://localhost:$puerto/health"; then
      echo "  OK  -> Swagger: http://localhost:$puerto/swagger"
      break
    fi
    sleep 1
  done
done

echo ""
echo "Todo levantado. Ctrl+C para detener."
echo "La salida de consola de cada servicio queda en src/<Servicio>/logs/consola.txt"
wait
