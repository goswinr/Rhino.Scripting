#!/bin/bash
# SessionStart hook for Claude Code cloud sessions:
# installs the .NET SDK (same major version as the CI in .github/workflows/build.yml),
# restores the local dotnet tools and the NuGet packages of both projects.
set -euo pipefail

# Only run in Claude Code cloud sessions, locally the SDK is already installed.
if [ "${CLAUDE_CODE_REMOTE:-}" != "true" ]; then
  exit 0
fi

DOTNET_CHANNEL="10.0"
DOTNET_DIR="$HOME/.dotnet"
PROJECT_DIR="${CLAUDE_PROJECT_DIR:-$(pwd)}"

export DOTNET_ROOT="$DOTNET_DIR"
export PATH="$DOTNET_DIR:$DOTNET_DIR/tools:$PATH"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

# Persist for all later Bash calls of this session
if [ -n "${CLAUDE_ENV_FILE:-}" ]; then
  {
    echo "export DOTNET_ROOT=\"$DOTNET_DIR\""
    echo "export PATH=\"$DOTNET_DIR:$DOTNET_DIR/tools:\$PATH\""
    echo "export DOTNET_CLI_TELEMETRY_OPTOUT=1"
    echo "export DOTNET_NOLOGO=1"
    echo "export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1"
  } >> "$CLAUDE_ENV_FILE"
fi

# Install the SDK only if no SDK of this channel is present yet (idempotent)
if ! dotnet --list-sdks 2>/dev/null | grep -q "^${DOTNET_CHANNEL%.0}\."; then
  echo "Installing .NET SDK $DOTNET_CHANNEL into $DOTNET_DIR ..."
  curl -sSL https://dot.net/v1/dotnet-install.sh -o /tmp/dotnet-install.sh
  bash /tmp/dotnet-install.sh --channel "$DOTNET_CHANNEL" --install-dir "$DOTNET_DIR" --no-path
  rm -f /tmp/dotnet-install.sh
fi
dotnet --version

cd "$PROJECT_DIR"

# fsdocs-tool from .config/dotnet-tools.json
dotnet tool restore

# NuGet packages for both projects (net48 reference assemblies come via NuGet on Linux)
dotnet restore ForEditing.fsproj
dotnet restore ForPublishing.fsproj
