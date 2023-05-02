#!/bin/bash

# Fail on any error.
set -e

export SHARED_PATH="${KOKORO_ARTIFACTS_DIR}/piper/google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/mediation"
export PLUGIN_PATH="${SHARED_PATH}/${ADAPTER_DIR}"

# 1. Prepare Unity tools
. "${SHARED_PATH}"/release/kokoro/scripts/setup.sh

# 2. Prepare gradle
gradlew="${KOKORO_ARTIFACTS_DIR}/piper/google3/third_party/gradle/wrapper_files/gradlew"
cp "${gradlew}" "${PLUGIN_PATH}/"

mkdir -p "${PLUGIN_PATH}"/gradle/wrapper

wrapper="${KOKORO_ARTIFACTS_DIR}/piper/google3/third_party/gradle/wrapper_files/gradle/wrapper/gradle-wrapper.jar"
cp "${wrapper}" "${PLUGIN_PATH}/gradle/wrapper/"

properties="${KOKORO_ARTIFACTS_DIR}/piper/google3/third_party/gradle/wrapper_files/gradle/wrapper/gradle-wrapper.properties"
cp "${properties}" "${PLUGIN_PATH}/gradle/wrapper/"

# 3. Build a plugin
cd "${PLUGIN_PATH}"
./gradlew makeZip

# 4. Move resulting zip into artifact folder
# if VERSION_NAME_SUFFIX is provided - we're building experimentals
# and we should put it into nested folder
location="${KOKORO_ARTIFACTS_DIR}/$([[ ! -z "$VERSION_NAME_SUFFIX" ]] && echo "$VERSION_NAME_SUFFIX")"
mv *.zip "${location}"
