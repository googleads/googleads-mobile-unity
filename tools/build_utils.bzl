"""
Utility rules for GoogleMobileAds Unity plugin build.
"""

def copybara_stripped_zip(
        name,
        srcs,
        workflow_name):
    """Generates a zip file using the given copybara workflow to strip the given srcs."""
    native.genrule(
        name = name,
        srcs = srcs,
        outs = ["%s.zip" % name],
        cmd = (
            """
            # Extract the additional needed binaries.
            cp $(location //tools/python:pystrip.par) /tmp/pystrip.par
            cp $(location //testing/leakr/parser) /tmp/leakr
            cp $(location //devtools/buildozer) /tmp/buildozer
            cp $(location //devtools/buildifier) /tmp/buildifier

            # Run copybara with a folder destination.
            HOME=$$PWD/$(@D) //java/com/google/devtools/copybara:copybara \
            $(location //java/com/google/android/libraries/admob/demo/unity/googlemobileads/opensource:copybara) \
            {workflow_name} --ignore-noop --pystrip-bin=/tmp/pystrip.par --leakr-bin=/tmp/leakr \
            --buildozer-bin=/tmp/buildozer --buildifier-bin=/tmp/buildifier \
            --folder-dir "$(@D)/out" ../

            # Create a zip package of the output.
            zip=$$(readlink -f $(location //third_party/zip));
            output=$$(readlink -f "$(@)");
            cd $(@D)/out;
            chmod +w *;
            $$zip -0qrXD $$output *;

            """
        ).format(
            workflow_name = workflow_name,
        ),
        # There are files unused but needed when loading copybara configuration.
        tools = [
            "//devtools/buildifier",
            "//devtools/buildozer",
            "//devtools/copybara/library:all_libraries",
            "//java/com/google/devtools/copybara",
            "//testing/leakr/common/dictionary:badfiles.dic",
            "//testing/leakr/common/dictionary:default.dic",
            "//testing/leakr/common/recipes:default.ftrcp",
            "//testing/leakr/parser",
            "//third_party/zip",
            "//java/com/google/android/libraries/admob/demo/unity/googlemobileads/opensource:copybara",
            "//java/com/google/android/libraries/admob/examples:common.bara.sky",
            "//tools/python:pystrip.par",
        ],
        visibility = ["//java/com/google/android/libraries/admob/demo/unity:__subpackages__"],
    )

def dll_sources(
        name,
        srcs):
    """Generates a filegroup using the name and depending on the eap build flag."""
    native.filegroup(
        name = "%s_files" % name,
        srcs = srcs,
    )

    copybara_stripped_zip(
        name = "%s_non_eap_source" % name,
        srcs = [
            "%s_files" % name,
        ],
        workflow_name = "to_folder_internal",
    )

    native.filegroup(
        name = "%s_dll_sources" % name,
        srcs = select({
            "eap": ["%s_files" % name],
            "//conditions:default": ["%s_non_eap_source.zip" % name],
        }),
    )
