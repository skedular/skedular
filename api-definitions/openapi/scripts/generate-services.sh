#!/usr/bin/env bash

set -euo pipefail

manifest="/openapi/services.manifest.tsv"

while IFS='|' read -r input namespace classname output; do
  if [[ -z "${input}" ]]; then
    continue
  fi

  nswag \
    openapi2cscontroller \
    /Input:"${input}" \
    /Namespace:"${namespace}" \
    /Classname:"${classname}" \
    /Output:"${output}" \
    /ControllerBaseClass:Microsoft.AspNetCore.Mvc.Controller \
    /AdditionalNamespaceUsages:Microsoft.AspNetCore.Mvc \
    /ControllerStyle:abstract \
    /HandleReferences:true \
    /ArrayType:System.Collections.Generic.IList \
    /DictionaryType:System.Collections.Generic.IDictionary \
    /UseActionResultType:true \
    /UseCancellationToken:true \
    /GenerateNullableReferenceTypes:true \
    /JsonLibrary:SystemTextJson \
    /ExcludedTypeNames:FileParameter

  sed -i '1iusing FileParameter = Microsoft.AspNetCore.Http.IFormFile;' "${output}"

  /app/publish/Skedularctl mcp-tool-generate \
    --input-file "${output}" \
    --output-file "${output}"
done < "${manifest}"
