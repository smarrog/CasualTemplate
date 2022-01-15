#!/usr/bin/env bash

set -Eeuo pipefail

case $0 in
    /*)
        cd "$(dirname $0)"
    ;;
    *)
        cd "$(dirname $(pwd)/$0)"
    ;;
esac

usage() {
  cat <<EOF
Usage: $(basename "${BASH_SOURCE[0]}") [OPTIONS]

Available options:

-h, --help            Print this help and exit
-i,                   Project names with message pack objects(allows pass multiple paths using ',', when -o must be empty). Generates all resolvers is empty.
--json                Pass json file with project names with message pack objects and its hashes. Format: [{"asmdef":"path","hash":"hash"}]
-o,                   Output directory. If empty, by default, looks for the resolver name, produced from the project name, in -i parameter. Doesn't allow multiple prms.
-e, --executable      Path to .NET executable file. Default: dotnet
-f, --force-recompile Force recompile project files. Unity mast be closed. Default: false
-d, --debug_mode      Resolver generating debug mode. Default: false
EOF
  exit
}

log() { 
  echo ""
  echo -e "-- ${1-}"
}

log_debug() {
  if [ "$debug_mode" == "true" ]; then
    printf "%s\n" "$1"
  fi
}

log_error() {
  echo ""
  >&2 echo -e "\033[0;31m-- ${1-}\033[0m"
  exit 1
}

generate_resolver() {
  local asmdef_path=$1
  local output_path=$2
  local formatter_name=$3

  log "Generating resolver: $asmdef_path --> $output_path - started"
  mpc_args=(-i "$asmdef_path" -o "$output_path" -r "$formatter_name")

  local debug_mpc_output=""
  
  if [ "$debug_mode" = true ]; then
    mpc_output=$(time "$executable_path" mpc "${mpc_args[@]}") || log_error "Error during generating $formatter_name:\n$mpc_output"
    debug_mpc_output="\n$mpc_output"
  else
    mpc_output=$("$executable_path" mpc "${mpc_args[@]}" 2>&1) || log_error "Error during generating $formatter_name:\n$mpc_output"
  fi
  log "Generating resolver: $asmdef_path --> $output_path - completed$debug_mpc_output"
}

MP_CATALOG_EXECUTABLE=./Tools/MessagePackTools/MessagePackTools.Catalog

inputs=""
output_path=""
input_json=""
executable_path="dotnet"
debug_mode=false
force_recompile=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    -h | --help) usage ;;
    -i)
      shift
      inputs=$(echo "$1" | tr ',' ' ')
      ;;
    --json)
      shift
      input_json="$1"
      ;;
    -o)
      shift
      output_path="$1"
      output_path=${output_path%/}
      ;;
    -e | --executable)
      shift
      executable_path="$1"
      ;;
    -f | --force-recompile)
      force_recompile=true
      ;;
    -d | --debug_mode)
      debug_mode=true
      MP_CATALOG_EXECUTABLE="$MP_CATALOG_EXECUTABLE -d"
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
  shift
done

if [ "$force_recompile" = true ]; then
  log "Recompiling project files..."
  unity -batchmode -nographics -quit -projectPath . -executeMethod UnityEditor.SyncVS.SyncSolution
  log "Recompiling project files completed"
fi

"$executable_path" tool restore > /dev/null
log_debug "Inputs: $inputs"

if [ -z "$input_json" ]; then
  log "Finding projects with message pack objects..."
#  dotnet build $MP_CATALOG_EXECUTABLE -v q > /dev/null
  raw_output=$("$executable_path" run -c Release --project $MP_CATALOG_EXECUTABLE 2>&1) || log_error "Error during finding projects with message pack objects:\n$raw_output"
  log_debug "$MP_CATALOG_EXECUTABLE raw mpc_output:"
  log_debug "$raw_output"
  
  input_json=$(echo "$raw_output" | grep -o '{[^}]*}')
else
  log "Using provided json: $input_json"
fi
# shellcheck disable=SC2001
asm_path_and_hash=$(echo "$input_json" | sed 's/[{"}]//g')
log "Found asmdefs: $asm_path_and_hash"    


has_inputs=$( [ -n "$inputs" ] && echo true || echo false )
if [ "$has_inputs" == "true" ]; then
  IFS=' ' read -r -a processing_projects <<< "$inputs"
else
  processing_projects=()
fi


asm_path_array=()
hash_array=()

IFS=',' read -r -a asm_path_and_hash_array <<< "$asm_path_and_hash"
for item in "${asm_path_and_hash_array[@]}"; do
    IFS=':' read -r asmdef_path hash <<< "$item"
    
    if [ "$has_inputs" == "true" ]; then
        match_found=false
        for project in "${processing_projects[@]}"; do
            asmdef_name="${project%.csproj}.asmdef"
            if [[ "$asmdef_path" == *"$asmdef_name"* ]]; then
                match_found=true
                break
            fi
        done

        if [ "$match_found" == "false" ]; then
            continue
        fi
    fi
    
    asm_path_array+=("$asmdef_path")
    hash_array+=("$hash")
done

log_debug "array size: ${#asm_path_array[@]}"
for item in "${asm_path_array[@]}"; do
  log_debug "$item"
done

log "Cleaning: removing ./obj"
rm -rf ./obj

running_pids=()
for i in "${!asm_path_array[@]}"; do
  {
    asmdef_path=${asm_path_array[i]}
    log_debug "Processing: $asmdef_path"
    asmdef_file_name=$(echo "$asmdef_path" | awk -F/ '{print $NF}')
    formatter_name="${asmdef_file_name%.asmdef}_Resolver"
    formatter_name=$(echo "$formatter_name" | awk -F. '{gsub(/\./, "_"); print}')  
    
    if [ -n "$output_path" ]; then
      path_to_resolver="${output_path%/}/$formatter_name.cs"
    else
      path_to_resolver="${asmdef_path%/*}/$formatter_name.cs"
      output_path="${asmdef_path%/*}"
    fi
    
    project_path="./Temp/mp_csprojs_generated/${asmdef_file_name%.asmdef}.csproj"
    generate_resolver "$project_path" "$path_to_resolver" "$formatter_name"

    hash_name=".$(echo "$asmdef_file_name" | awk -F. '{gsub(/\./, "_"); gsub("_asmdef","_Hash.txt"); print}')"
    path_to_hash="${asmdef_path%/*}/$hash_name"
    log "Writing hash to: $path_to_hash"
    echo "${hash_array[i]}" > "$path_to_hash"
  } & running_pids+=($!)
done

was_errors=false
for pid in "${running_pids[@]}"; do
  wait $pid || was_errors=true
done

if [ "$was_errors" = true ]; then
  log_error "Completed with errors"
fi

log "COMPLETED"
