#!/usr/bin/env bash

set -euo pipefail

manifest="/openapi/clients.manifest.tsv"

while IFS='|' read -r input namespace classname output; do
  if [[ -z "${input}" ]]; then
    continue
  fi

  nswag \
    openapi2csclient \
    /Input:"${input}" \
    /Namespace:"${namespace}" \
    /Classname:"${classname}" \
    /Output:"${output}" \
    /GenerateClientClasses:true \
    /OperationGenerationMode:SingleClientFromOperationId \
    /GenerateClientInterfaces:true \
    /InjectHttpClient:true \
    /UseBaseUrl:false \
    /GenerateOptionalParameters:true \
    /GenerateJsonMethods:false \
    /ArrayType:System.Collections.Generic.IList \
    /DictionaryType:System.Collections.Generic.IDictionary \
    /ParameterDateTimeFormat:"yyyy'-'MM'-'dd'T'HH':'mm':'ssK" \
    /GenerateDtoTypes:true \
    /GenerateNullableReferenceTypes:true \
    /JsonLibrary:SystemTextJson

done < "${manifest}"
