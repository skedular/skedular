#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

if [[ "$#" -gt 0 ]]; then
  apps=("$@")
else
  apps=(webapp)
fi
required=(infrastructure src public)

for app in "${apps[@]}"; do
  app_path="$root/src/web/apps/$app"
  if [[ ! -d "$app_path" ]]; then
    echo "MISSING_APP: $app_path"
    exit 1
  fi

  for item in "${required[@]}"; do
    if [[ ! -e "$app_path/$item" ]]; then
      echo "MISSING_PATH: $app_path/$item"
      exit 1
    fi
  done

  echo "OK: $app_path"
done

echo "PASS: all main app structures present"
