#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

apps=()
if [[ "$#" -gt 0 ]]; then
  apps=("$@")
else
  apps=(webapp)
fi

pkg="@skedular/ui"
expected=""

for app in "${apps[@]}"; do
  file="$root/web/apps/$app/package.json"
  if [[ ! -f "$file" ]]; then
    echo "MISSING: $file"
    exit 1
  fi

  version=$(grep -Eo '"@skedular/ui"\s*:\s*"[^"]+"' "$file" | head -n1 | sed -E 's/.*:\s*"([^"]+)"/\1/')
  if [[ -z "$version" ]]; then
    echo "MISSING_DEP: $file does not declare $pkg"
    exit 1
  fi

  if [[ -z "$expected" ]]; then
    expected="$version"
  elif [[ "$version" != "$expected" ]]; then
    echo "MISMATCH: $file has $version, expected $expected"
    exit 1
  fi

  echo "OK: $file -> $version"
done

echo "PASS: $pkg versions are aligned ($expected)"
