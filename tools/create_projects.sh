#!/bin/bash -eu

# Copyright 2020 Google LLC
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#      http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.


help() {
echo "This script runs the swig command to generate C# content.
Usage: build_plugin.sh OPTIONS
OPTIONS:
  -a <assemblyInfoTemplate>
  -c <confuserExTemplate>
  -h                            Prints this message.
  -n <project_name>             What the plugin will be called.
  -o <out_dir>                  Where the outputs are dumped.
  -p <project_file_template>
  -r                            Rename, if set renames output with '_unobfs' suffix
  -s <source_files_string>      String with all source files to include in project
  -v <visible_to>               The other projects we want to make it visible.
" >&2
}

# See help() for documentation.
main() {
    RENAME=0;
    CONFUSER_EX_IN="";
    VISIBLE_TO="";
    while getopts ":a:c:hn:o:p:rs:v:" opt
    do
        case "${opt}" in
            a)  ASSEMBLY_INFO_IN="${OPTARG}";;
            c)  CONFUSER_EX_IN="${OPTARG}";;
            h)  help;exit 1;;
            n)  PROJECT_NAME="${OPTARG}";;
            o)  OUT_DIR="${OPTARG}";;
            p)  PROJECT_FILE_IN="${OPTARG}";;
            r)  RENAME=1;;
            s)  SOURCE_FILES_IN="${OPTARG}";;
            v)  VISIBLE_TO="${OPTARG}";;
            \?) echo "Invalid Option: -${OPTARG}" >&2; exit 1;;
            :)  echo "Option -${OPTARG} requires an argument." >&2; exit 1;;
            *)  echo "Unimplemented Option: -${OPTARG}" >&2; exit 1;;
        esac
    done

    args_good=1
    if [[ -z "${OUT_DIR+x}" ]]; then
        echo "-o required."
        args_good=0
    fi
    if [[ -z "${PROJECT_NAME+x}" ]]; then
        echo "-n required."
        args_good=0
    fi
    if [[ -z "${SOURCE_FILES_IN+x}" ]]; then
        echo "-s required."
        args_good=0
    fi
    if [[ -z "${ASSEMBLY_INFO_IN+x}" ]]; then
        echo "-a required."
        args_good=0
    fi
    if [[ -z "${PROJECT_FILE_IN+x}" ]]; then
        echo "-p required."
        args_good=0
    fi
    if [[ $((args_good)) -eq 0 ]]; then
        help
        exit 1;
    fi

    PROJECT_FILE_OUT="${OUT_DIR}/${PROJECT_NAME}.csproj"
    ASSEMBLY_INFO_OUT="${OUT_DIR}/${PROJECT_NAME}/AssemblyInfo.cs"
    ASSEMBLY_INFO_OUT_TEMP="${OUT_DIR}/${PROJECT_NAME}/AssemblyInfo.cs.tmp"
    CONFUSER_EX_OUT="${OUT_DIR}/${PROJECT_NAME}.crproj"
    UNOBFS_DLL_NAME="${PROJECT_NAME}_unobfs.dll"

    # set up a temporary build location
    mkdir -p "${OUT_DIR}/${PROJECT_NAME}"
    sed "s/%PROJECT_NAME%/${PROJECT_NAME}/" "${ASSEMBLY_INFO_IN}" > \
      "${ASSEMBLY_INFO_OUT}"
    if [[ -n $VISIBLE_TO ]]; then
      for visible_to in $(echo "${VISIBLE_TO}" | sed "s/,/ /g"); do
        cp "${ASSEMBLY_INFO_OUT}" "${ASSEMBLY_INFO_OUT_TEMP}"
        local VISIBILITY="[assembly: InternalsVisibleTo(\"${visible_to}\")]"
        sed "s@%INTERNALS_VISIBLE_TO%@%INTERNALS_VISIBLE_TO%\\n${VISIBILITY}@g" \
          "${ASSEMBLY_INFO_OUT_TEMP}" > "${ASSEMBLY_INFO_OUT}"
      done
    fi
    cp "${ASSEMBLY_INFO_OUT}" "${ASSEMBLY_INFO_OUT_TEMP}"
    sed "s@// %INTERNALS_VISIBLE_TO%@@g" "${ASSEMBLY_INFO_OUT_TEMP}" > \
      "${ASSEMBLY_INFO_OUT}"
    rm "${ASSEMBLY_INFO_OUT_TEMP}"

    if [[ -n $CONFUSER_EX_IN ]]; then
      cat "${CONFUSER_EX_IN}" | \
      sed "s@<!--%FILE_NAME%-->@${UNOBFS_DLL_NAME}@g;" > \
           "${CONFUSER_EX_OUT}"
    fi
    echo "Generated the following to ${OUT_DIR}..." >&2
    (
      cd "${OUT_DIR}"
      find . -type f
    ) >&2

    find "${OUT_DIR}" -type f | xargs -I@ chmod 770 "@"

    # Finds all of the generated files from swig, and creates include directives
    # in the template csproj file.
    local compile_statements="";
    for f in $(echo "${SOURCE_FILES_IN}" | sed "s/,/ /g"); do
        compile_statements="${compile_statements}<Compile Include=\"${f}\" /\>\\n    "
    done
    # echo "Compiling: ${compile_statements}";
    local sed_rename="";
    if [[ $((RENAME)) -eq 1 ]]; then
        sed_rename="s@<!--%RENAME_START%@<!-- Rename start -->@g;
                    s@%RENAME_END%-->@<!-- Rename end -->@g;"
    fi
    cat "${PROJECT_FILE_IN}" | \
    sed "${sed_rename}
         s@<!--%PROJECT_NAME%-->@${PROJECT_NAME}@g;
         s@<!--%FILE_LIST%-->@${compile_statements}@g;" > \
         "${PROJECT_FILE_OUT}"

}

main "$@"
