#!/bin/bash
set -euo pipefail

DRY_RUN="${DRY_RUN:-true}"
if [[ "${DRY_RUN}" != "true" && "${DRY_RUN}" != "false" ]]; then
  echo "ERROR: DRY_RUN must be 'true' or 'false' (got '${DRY_RUN}')." >&2
  exit 1
fi

REPO_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "${REPO_DIR}"

echo "=== Building and Releasing from: ${REPO_DIR} (DRY_RUN=${DRY_RUN}) ==="

PACKAGE_NAME="googleads-mobile-unity"
PACKAGE_VERSION="$(sed -n 's/^Version[[:space:]]\+\([0-9.]\+\).*/\1/p' ChangeLog.txt | head -n 1)"
if [[ -z "${PACKAGE_VERSION}" ]]; then
  echo "ERROR: Could not extract version from ChangeLog.txt" >&2
  exit 1
fi
echo "Package: ${PACKAGE_NAME}, Version: ${PACKAGE_VERSION}"

rm -rf artifacts/
mkdir -p artifacts/
if [[ -f "GoogleMobileAds.unitypackage" ]]; then
  cp "GoogleMobileAds.unitypackage" "artifacts/GoogleMobileAds-v${PACKAGE_VERSION}.unitypackage"
else
  git archive --format=tar.gz --prefix="${PACKAGE_NAME}-${PACKAGE_VERSION}/" HEAD > "artifacts/${PACKAGE_NAME}-${PACKAGE_VERSION}.tar.gz"
fi

EXIT_GATE_PROJECT="oss-exit-gate-prod"
EXIT_GATE_LOCATION="us"
EXIT_GATE_REPOSITORY="googleads-unity--githubreleases"

if gcloud artifacts packages describe "${PACKAGE_NAME}" \
    --project="${EXIT_GATE_PROJECT}" \
    --location="${EXIT_GATE_LOCATION}" \
    --repository="${EXIT_GATE_REPOSITORY}" &>/dev/null; then
  echo "=== Deleting existing package '${PACKAGE_NAME}' from Exit Gate staging repository ==="
  gcloud artifacts packages delete "${PACKAGE_NAME}" \
    --project="${EXIT_GATE_PROJECT}" \
    --location="${EXIT_GATE_LOCATION}" \
    --repository="${EXIT_GATE_REPOSITORY}" \
    --quiet
fi

echo "=== Uploading release artifacts to Exit Gate staging repository ==="
gcloud artifacts generic upload \
  --project="${EXIT_GATE_PROJECT}" \
  --location="${EXIT_GATE_LOCATION}" \
  --repository="${EXIT_GATE_REPOSITORY}" \
  --package="${PACKAGE_NAME}" \
  --version="${PACKAGE_VERSION}" \
  --source-directory=artifacts/

if [[ "${DRY_RUN}" == "true" ]]; then
  echo "=== DRY_RUN is enabled. Skipping manifest upload to Exit Gate. ==="
  exit 0
fi

cat <<EOF > manifest.json
{
  "publish_all": false,
  "publishing_groups": [
    {
      "packages": [
        {
          "name": "${PACKAGE_NAME}",
          "version": "${PACKAGE_VERSION}"
        }
      ]
    }
  ]
}
EOF

EXIT_GATE_BUCKET="gs://oss-exit-gate-prod-projects-bucket/googleads-unity/githubreleases/manifests"
MANIFEST_NAME="manifest-$(date +%Y%m%d%H%M%S).json"

echo "=== Uploading manifest to ${EXIT_GATE_BUCKET}/${MANIFEST_NAME} ==="
gcloud storage cp manifest.json "${EXIT_GATE_BUCKET}/${MANIFEST_NAME}"
