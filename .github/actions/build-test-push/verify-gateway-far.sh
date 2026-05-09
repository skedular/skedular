#!/usr/bin/env bash
# Verify that a gateway.far file is a valid, fully readable zip archive.
#
# Usage:
#   verify-gateway-far.sh <path-to-gateway.far> [required=true|false]
#
# Exits with code 1 if the file is missing (when required=true) or corrupted.

set -euo pipefail

path="${1:-}"
required="${2:-false}"

if [[ -z "$path" ]]; then
    echo "Usage: verify-gateway-far.sh <path> [required=true|false]" >&2
    exit 1
fi

if [[ ! -f "$path" ]]; then
    if [[ "$required" == "true" ]]; then
        echo "ERROR: gateway.far not found at $path" >&2
        exit 1
    else
        echo "gateway.far not present at $path — skipping check"
        exit 0
    fi
fi

if unzip -t "$path" > /dev/null 2>&1; then
    count=$(unzip -l "$path" 2>/dev/null | awk 'END{print NR-4}')
    echo "gateway.far OK: $count entries, all readable ($path)"
else
    echo "ERROR: gateway.far is corrupt or not a valid zip: $path" >&2
    exit 1
fi
