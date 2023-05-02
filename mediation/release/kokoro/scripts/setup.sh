#!/bin/bash
# Display commands to stderr.
set -e

function die { echo "$@" >&2; exit 1; }

export UMB="${KOKORO_PIPER_DIR}/google3/devtools/unity3d/umb/unity_master_build.py"
readonly UMBROOT="${KOKORO_ARTIFACTS_DIR}/umbroot"
export UMBROOT

# Copy Unity from NFS to local file system
python3 "${KOKORO_PIPER_DIR}/google3/vr/tools/umb_helpers/copy_unity.py" -u "${UNITY_VER}"

# Get UNITY_EXE for use in gradlew call
if [[ -z "${UNITY_VER}" ]]; then
  version_arg=""
  UNITY_EXE_PREFIX="$(python3 "${UMB}" -A | grep '/' | sort -nr | head -n 1 | awk '{print $2}' )"
  echo "Note: Using latest installed Unity version.  This may be a beta."
else
  version_arg="-D unity_ver=${UNITY_VER}"
  UNITY_EXE_PREFIX="$(python3 "${UMB}" -A | grep '/' | grep "${UNITY_VER}" | sort -nr | head -n 1 | awk '{print $2}' )"
fi

export UNITY_EXE="${UNITY_EXE_PREFIX}/Contents/MacOS/Unity"

# If UNITY_VER is set, check that UNITY_EXE contains UNITY_VER.
if [[ -n "${UNITY_VER}" && ${UNITY_EXE} != *"${UNITY_VER}"* ]]; then
  die "Error: Unity path doesn't match requested Unity version."
fi
