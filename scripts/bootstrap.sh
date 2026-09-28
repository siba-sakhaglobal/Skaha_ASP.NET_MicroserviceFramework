#!/usr/bin/env bash
# Bootstraps a fresh clone of the SakhaERP framework for local development.
#
# Shared libraries (SakhaERP.Shared.*) are consumed by services via PackageReference,
# never ProjectReference (see docs/architecture/framework-rules.md). Until Azure Artifacts
# is wired up, this script packs them into the local NuGet feed declared in nuget.config
# (./packages) so `dotnet restore` on any service resolves them from there.
set -euo pipefail
cd "$(dirname "${BASH_SOURCE[0]}")/.."

echo "==> Packing shared libraries into ./packages"
mkdir -p packages
for proj in shared/*/*.csproj; do
  dotnet pack "$proj" -c Release -o packages
done

echo "==> Restoring SakhaERP.sln"
dotnet restore SakhaERP.sln

echo "==> Done. Run 'docker compose up -d' then 'dotnet run --project services/inventory-service/src/InventoryService.Api' to start the reference service."
echo "    Added a new shared library or service? Run 'dotnet sln SakhaERP.sln add <path-to-csproj>' to include it."
