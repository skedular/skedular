#!/usr/bin/env sh

set -e

REPO="unityhubio/unityhubio"

echo "Deleting all GitHub Actions workflow runs for ${REPO}..."

for status in completed failure cancelled success; do
  ids=$(gh run list --repo "$REPO" --limit 1000 --status "$status" --json databaseId -q '.[].databaseId' 2>/dev/null || true)
  if [ -n "$ids" ]; then
    echo "$ids" | xargs -I {} gh run delete --repo "$REPO" {}
  fi
done

echo "Done."
