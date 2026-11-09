#!/usr/bin/env sh

set -eu

generator="$(CDPATH= cd -- "$(dirname -- "$0")/.." && pwd)/generate.sh"
test_dir="$(mktemp -d)"
cleanup() {
    rm -f "$test_dir/docker" "$test_dir/docker.log" "$test_dir/output.log"
    rmdir "$test_dir"
}
trap cleanup EXIT

cat > "$test_dir/docker" <<'MOCK'
#!/usr/bin/env sh
set -eu
printf '%s\n' "$*" >> "$DOCKER_TEST_LOG"
if [ "$1" = build ]; then
    case "$*" in
        *clients.Dockerfile*) status="$FAIL_CLIENTS" ;;
        *services.Dockerfile*) status="$FAIL_SERVICES" ;;
        *) exit 2 ;;
    esac
    sleep 0.1
    printf 'build-completed %s\n' "$*" >> "$DOCKER_TEST_LOG"
    exit "$status"
fi
MOCK
chmod +x "$test_dir/docker"

for scenario in success clients-fail services-fail both-fail; do
    FAIL_CLIENTS=0
    FAIL_SERVICES=0
    case "$scenario" in
        clients-fail) FAIL_CLIENTS=1 ;;
        services-fail) FAIL_SERVICES=1 ;;
        both-fail) FAIL_CLIENTS=1; FAIL_SERVICES=1 ;;
    esac
    export FAIL_CLIENTS FAIL_SERVICES
    DOCKER_TEST_LOG="$test_dir/docker.log"
    export DOCKER_TEST_LOG
    : > "$DOCKER_TEST_LOG"

    result=0
    PATH="$test_dir:$PATH" sh "$generator" > "$test_dir/output.log" 2>&1 || result=$?

    if [ "$(grep -c '^build-completed ' "$DOCKER_TEST_LOG")" -ne 2 ]; then
        echo "FAIL: $scenario did not wait for both builds." >&2
        exit 1
    fi

    if [ "$scenario" = success ]; then
        if [ "$result" -ne 0 ] || [ "$(grep -c '^cp ' "$DOCKER_TEST_LOG")" -ne 2 ]; then
            echo "FAIL: successful builds did not extract both outputs." >&2
            exit 1
        fi
    elif [ "$result" -eq 0 ] || grep -Eq '^(create|cp) ' "$DOCKER_TEST_LOG"; then
        echo "FAIL: $scenario extracted stale output or reported success." >&2
        exit 1
    fi
    printf 'PASS: %s\n' "$scenario"
done
