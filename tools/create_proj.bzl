"""Support rules for creating DLLs for Unity projects."""

load(
    "//firebase/app/client/unity:build_mono_project.bzl",
    "build_mono_project",
)

def create_project(
        name,
        srcs,
        deps,
        obfuscate = False,
        visible_to = ""):
    """Creates Unity build rules for given source sets and dependencies.

    Args:
      name: Name of the rule.
      srcs: Sources to build.
      deps: Dependencies for the rule.
      obfuscate: Set to true to obfuscate the DLL using ConfuserEx. Defaults to False.
      visible_to: Optional space or comma separated list of assemblies to expose internals to.
    """
    assembly_name = name
    obfs_outs = []
    if obfuscate:
        obfs_outs = ["%s.crproj" % assembly_name]

    native.genrule(
        name = "%s_proj" % assembly_name,
        srcs = srcs,
        outs = [
            "%s.csproj" % assembly_name,
            "%s/AssemblyInfo.cs" % assembly_name,
        ] + obfs_outs,
        cmd = ("""
                set -e;

                SOURCES="{assembly_name}/AssemblyInfo.cs ";
                for SRC in $(SRCS); do
                  if [[ $${{SRC}} =~ \\_source.zip$$ ]]; then
                    SOURCES="$$SOURCES$$(zipinfo -1 \"$${{SRC}}\") ";
                  elif [[ -f $${{SRC}} ]]; then
                    SOURCES="$$SOURCES$$(echo $$SRC | \
                    sed -r 's@(blaze-out/k8-opt/genfiles/)?java/com/google/android/libraries/admob/demo/unity/googlemobileads/@@' \
                    ) ";
                  fi
                done
                OBFS_CMD="";
                if [[ "{obfuscate}" = true ]]; then
                  OBFS_CMD="-r -c $(location tools/ConfuserexTemplate.crproj)";
                fi
                $(location tools/create_projects.sh) \
                -o $(@D) \
                -n {assembly_name} \
                -a $(location {assembly_info_template}) \
                -p $(location tools/Template.csproj) \
                -v '{visible_to}' \
                -s "$$SOURCES" \
                $$OBFS_CMD;
                """.format(
            assembly_info_template = "tools/AssemblyInfoTemplate.cs",
            visible_to = visible_to,
            assembly_name = assembly_name,
            obfuscate = ("true" if obfuscate else "false"),
        )),
        tools = [
            "tools/create_projects.sh",
            "tools/Template.csproj",
            "tools/ConfuserexTemplate.crproj",
            "tools/AssemblyInfoTemplate.cs",
        ],
    )

    dll_out = ("%s_unobfs.dll" if obfuscate else "%s.dll") % assembly_name

    build_mono_project(
        name = "%s_dll" % assembly_name,
        srcs = srcs + [
            "%s/AssemblyInfo.cs" % assembly_name,
        ],
        outs = [
            dll_out,
        ],
        build_config = "Release",
        project = "%s.csproj" % assembly_name,
        warning_level = 3,
        deps = deps,
    )

    if obfuscate:
        native.genrule(
            name = "%s_obfuscated_dll" % assembly_name,
            srcs = [
                "%s_dll" % assembly_name,
                "%s_proj" % assembly_name,
                "//third_party/ConfuserEx:confuser_ex",
            ] + deps,
            outs = [
                "%s.dll" % assembly_name,
                "%s_symbols.map" % assembly_name,
            ],
            cmd = (
                """
               set -e;
               mkdir -p /tmp/Confuser/output;
               temp_dir=/tmp/Confuser;
               for src in $(SRCS); do
                 cp -f \"$$src\" \"$$temp_dir\";
               done;
               chmod -R +w \"$$temp_dir\";
               wine_temp_dir=\"$$(echo \"$$temp_dir\" | tr '/' '\\\\')\";
               proj_wine_temp_dir=z:\"$$wine_temp_dir\"\\\\{assembly_name}.crproj;
               $(location //third_party/msvc:run_exe) \
               \"$$temp_dir\"/Confuser.CLI.exe \
               z:\"$$wine_temp_dir\"\\\\{assembly_name}.crproj;
               mv \"$$temp_dir\"/output/{assembly_name}_unobfs.dll $(location {assembly_name}.dll);
               mv \"$$temp_dir\"/output/symbols.map $(location {assembly_name}_symbols.map);
               """.format(
                    assembly_name = assembly_name,
                )
            ),
            tools = [
                "//third_party/msvc:everything_file_group",
                "//third_party/msvc:run_exe",
            ],
        )
