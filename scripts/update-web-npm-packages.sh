#!/usr/bin/env sh

set -e
set -x

original_dir=$(pwd)
trap 'cd "$original_dir"' EXIT

repo_root=$(cd "$(dirname "${0}")/../src" && pwd)
web_root="$repo_root/web"

update_workspace() {
    workspace_dir="$1"
    workspace_name=$(basename "$workspace_dir")

    case "$workspace_name" in
        *event-catalog*)
            echo "Skipping $workspace_dir"
            return
            ;;
    esac

    cd "$workspace_dir"
    ncu -u
}

update_workspace "$web_root"

for workspace_dir in "$web_root"/apps/* "$web_root"/packages/*; do
    [ -d "$workspace_dir" ] || continue
    [ -f "$workspace_dir/package.json" ] || continue

    update_workspace "$workspace_dir"
done