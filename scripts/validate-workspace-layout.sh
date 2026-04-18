#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ "$#" -gt 0 ]]; then
  apps=("$@")
else
  apps=(webapp)
fi
workspaces=(staging common_resources production)

for app in "${apps[@]}"; do
  base="$root/web/apps/$app/infrastructure/workspaces"
  if [[ ! -d "$base" ]]; then
    echo "MISSING_WORKSPACE_ROOT: $base"
    exit 1
  fi

  for ws in "${workspaces[@]}"; do
    if [[ ! -d "$base/$ws" ]]; then
      echo "MISSING_WORKSPACE: $base/$ws"
      exit 1
    fi
  done

  echo "OK: $base"
done

echo "PASS: workspace layout present for all main apps"
