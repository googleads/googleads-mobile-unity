local include "gmscore/release/blueprint/gmscore_flags.ncl";
include "releasetools/rapid/ncl/rapid_config.ncl";
include "devtools/blueprint/ncl/blueprint_file.ncl";

namespace AdMobUnityRapid {

UNITY_PLUGIN_IOS_FLAGS = [
  "--config=ios_common",
  "--ios_multi_cpus=arm64,x86_64",
  "--ios_minimum_os=14.0",
];

DEFAULT_TARGET_UNITY_VERSION = "6000.3";
DEFAULT_ANDROID_TARGET_SDK = "36";
DEFAULT_IOS_VERSION = "16.0";
DEFAULT_PROJECT_NAME = "HelloWorld";
DEFAULT_ADMOB_ANDROID_APP_ID = "ca-app-pub-3940256099942544~3347511713";
DEFAULT_ADMOB_IOS_APP_ID = "ca-app-pub-3940256099942544~1458002511";

FORCE_BUILD_UNITS_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "force_build_units",
  label = "Force Build Units",
  default_value = true,
  modifiable = false,
  description = "Whether to run Blaze build on buildable units before triggering the Kokoro build. Intended when the units are not automatically built during the candidate creation.",
);
ADMOB_ANDROID_APP_ID_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Simple(
  name = "admob_android_app_id",
  label = "AdMob Android App ID",
  default_value = DEFAULT_ADMOB_ANDROID_APP_ID,
  modifiable = true,
  description = "The unique Android app identifier assigned to your app in the AdMob frontend.",
);
ADMOB_IOS_APP_ID_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Simple(
  name = "admob_ios_app_id",
  label = "AdMob iOS App ID",
  default_value = DEFAULT_ADMOB_IOS_APP_ID,
  modifiable = true,
  description = "The unique iOS app identifier assigned to your app in the AdMob frontend.",
);
USE_UNITY_PACKAGE_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "use_unity_package",
  label = "Use Unity Package",
  default_value = false,
  modifiable = true,
  description = "Whether the Unity plugin should be imported or tested using the Unity package or the UPM dependency.",
);
TARGET_DECAGON_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "target_decagon",
  label = "Target Decagon",
  default_value = false,
  modifiable = true,
  description = "Whether the Unity plugin should be built for Decagon, as opposed to Nonagon (Android only).",
);
TARGET_DECAGON_WORKFLOW_PARAMETER_TRUE = ::Rapid::WorkflowParameter::Bool(
  name = "target_decagon",
  label = "Target Decagon",
  default_value = true,
  modifiable = false,
);
USE_CACHES_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "use_caches",
  label = "Use Caches",
  default_value = false,
  modifiable = true,
  description = "Whether the Unity and Gradle (if Android is selected) caches should be used to speed up the build. It must be enabled for offline builds.",
);
BUILD_OFFLINE_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "build_offline",
  label = "Build Offline",
  default_value = false,
  modifiable = true,
  description = "Whether the Unity plugin should be built offline (if so, caches must be allowed).",
);
FOR_IOS_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::Bool(
  name = "for_ios",
  label = "For iOS",
  default_value = false,
  modifiable = true,
  description = "Whether the Unity plugin should be built for iOS (IPA), as opposed to Android (APK).",
);
PLATFORMS_WORKFLOW_PARAMETER = ::Rapid::WorkflowParameter::MultiSelectEnum(
  name = "platforms",
  label = "Platform(s)",
  possible_values = ["Android", "iOS"],
  default_values = ["Android", "iOS"],
  modifiable = true,
  description = "The platforms to build for.",
);
SHARED_BUILD_AND_TEST_WORKFLOW_PARAMETERS = [
  ::Rapid::WorkflowParameter::SingleSelectEnum(
    name = "target_unity_ver",
    label = "Target Unity Version",
    // LINT.IfChange
    possible_values = ["6000.3", "6000.1", "2022.3", "2021.3"],
    // LINT.ThenChange(//depot/google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/unity/build_all_unity_apks_and_ipas.pp)
    default_value = DEFAULT_TARGET_UNITY_VERSION,
    modifiable = true,
    description = "Unity version for which to build or test the APK.",
  ),
  ::Rapid::WorkflowParameter::Simple(
    name = "android_target_sdk",
    label = "Android Target SDK",
    default_value = DEFAULT_ANDROID_TARGET_SDK,
    modifiable = true,
    description = "Android target SDK version for which to build or test the APK.",
  ),
  ::Rapid::WorkflowParameter::Simple(
    name = "ios_version",
    label = "iOS Version",
    default_value = DEFAULT_IOS_VERSION,
    modifiable = true,
    description = "iOS version for which to build or test the IPA (e.g., '16.0'). It must comply with Unity iOS and Apple requirements.",
  ),
  ::Rapid::WorkflowParameter::SingleSelectEnum(
    name = "project_name",
    label = "Project Name",
    // LINT.IfChange
    possible_values = ["HelloWorld"],
    // LINT.ThenChange(//depot/google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/unity/build_all_unity_apks_and_ipas.pp)
    default_value = DEFAULT_PROJECT_NAME,
    modifiable = true,
    description = "Sample project name used to build or test the APK.",
  ),
] + [USE_UNITY_PACKAGE_WORKFLOW_PARAMETER];

// Dev variant.
ADMOB_UNITY_PLUGIN_BUILDABLE_UNIT = ::blueprint::BuildableUnit(
  // LINT.IfChange
  name = "admob_unity_plugin_build",
  // LINT.ThenChange(//depot/google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/unity/build_unity_apk_or_ipa.pp, //depot/google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/raycast/build_raycast_apk_or_ipa.pp)
  build_patterns = [
    "//java/com/google/android/libraries/admob/demo/unity/googlemobileads:unitypackage_public",
    "//java/com/google/android/libraries/admob/demo/unity/googlemobileads:upm_package",
  ],
  build_flags = GMSCoreFlags::SDK_BUILD_FLAGS + UNITY_PLUGIN_IOS_FLAGS
    + ["--define=build_type=dev"],
  continuous_build_email = ::blueprint::ContinuousBuildEmailInfo(
    build_cop_email_addrs = ["vkini@google.com", "jochac@google.com"]),
  enable_continuous_build = true,
  enable_release = true,  // Enabled so it can be pulled by GMS Core during FDR
);

FORCE_BUILD_UNITY_APK_WORKFLOW_NAME = "Force Build Unity APK";

// This workflow is used by GMA SDK Nonagon Granular and FDR release processes
// (Android).
FORCE_BUILD_UNITY_APK_NONAGON_WORKFLOW = ::Rapid::Workflow::CustomWorkflow(
  name = FORCE_BUILD_UNITY_APK_WORKFLOW_NAME,
  config_path = "google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/unity/build_unity_apk_or_ipa.pp",
  parameters = SHARED_BUILD_AND_TEST_WORKFLOW_PARAMETERS + [
    ADMOB_ANDROID_APP_ID_WORKFLOW_PARAMETER,
    USE_CACHES_WORKFLOW_PARAMETER,
    FORCE_BUILD_UNITS_WORKFLOW_PARAMETER,
  // The `target_decagon` argument is false by default in the workflow, so we
  // don't need to pass it explicitly.
  ],
  description = "Builds the requested Unity APK, forcing the plugin to be built from source.",
);

// This workflow is used by GMA SDK Decagon Gradler release process (Android).
FORCE_BUILD_UNITY_APK_DECAGON_WORKFLOW = ::Rapid::Workflow::CustomWorkflow(
  name = FORCE_BUILD_UNITY_APK_WORKFLOW_NAME,
  config_path = "google3/third_party/java_src/gma_sdk_mediation/release/rapid/workflows/unity/build_unity_apk_or_ipa.pp",
  parameters = SHARED_BUILD_AND_TEST_WORKFLOW_PARAMETERS + [
    ADMOB_ANDROID_APP_ID_WORKFLOW_PARAMETER,
    USE_CACHES_WORKFLOW_PARAMETER,
    FORCE_BUILD_UNITS_WORKFLOW_PARAMETER,
    TARGET_DECAGON_WORKFLOW_PARAMETER_TRUE,
  ],
  description = "Builds the requested Unity APK, forcing the plugin to be built from source.",
);

}  // namespace AdMobUnityRapid
