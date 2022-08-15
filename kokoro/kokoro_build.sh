#!/bin/bash
# Fail on any error.
set -e
# Display commands being run.
# WARNING: please only enable 'set -x' if necessary for debugging, and be very
#  careful if you handle credentials (e.g. from Keystore) with 'set -x':
#  statements like "export VAR=$(cat /tmp/keystore/credentials)" will result in
#  the credentials being printed in build logs.
#  Additionally, recursive invocation with credentials as command-line
#  parameters, will print the full command, with credentials, in the build logs.
# set -x
# Code under depot is checked out to ${KOKORO_ARTIFACTS_DIR}/piper.
cd "${KOKORO_ARTIFACTS_DIR}/piper/google3/java/com/google/android/libraries/admob/demo/unity/googlemobileads/kokoro"
./build.sh
