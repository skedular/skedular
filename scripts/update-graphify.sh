#!/usr/bin/env bash
set -u

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"

if ! command -v graphify >/dev/null 2>&1; then
  echo "graphify command not found" >&2
  exit 127
fi

declare -a GRAPHIFY_DIRS=()
while IFS= read -r graphify_dir; do
  GRAPHIFY_DIRS+=("${graphify_dir}")
done < <(
  find "${REPO_ROOT}" \
    -path "${REPO_ROOT}/.git" -prune -o \
    -type d -name graphify-out -print | sort
)

if [[ ${#GRAPHIFY_DIRS[@]} -eq 0 ]]; then
  echo "No graphify-out directories found under ${REPO_ROOT}"
  exit 0
fi

declare -a PIDS=()
declare -a TARGETS=()

for graphify_dir in "${GRAPHIFY_DIRS[@]}"; do
  target_dir="$(dirname "${graphify_dir}")"
  relative_target="${target_dir#"${REPO_ROOT}/"}"

  if [[ "${target_dir}" == "${REPO_ROOT}" ]]; then
    relative_target="."
  fi

  echo "Updating graphify graph in ${relative_target}"
  (cd "${target_dir}" && graphify update) &
  PIDS+=("$!")
  TARGETS+=("${relative_target}")
done

FAILED=0

for index in "${!PIDS[@]}"; do
  if ! wait "${PIDS[index]}"; then
    echo "Failed to update graphify graph in ${TARGETS[index]}" >&2
    FAILED=1
  fi
done

exit "${FAILED}"
