/**
 * @generated SignedSource<<93117dc559860ea0c659035d589d698e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateZoneInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type zoneCard_updateZoneMutation$variables = {
  input: UpdateZoneInput;
};
export type zoneCard_updateZoneMutation$data = {
  readonly updateZone: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type zoneCard_updateZoneMutation = {
  response: zoneCard_updateZoneMutation$data;
  variables: zoneCard_updateZoneMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationTagPayload",
    "kind": "LinkedField",
    "name": "updateZone",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "organizationTag",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "zoneCard_updateZoneMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "zoneCard_updateZoneMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4e679a3bed84fbbb20f1be949a25566e",
    "id": null,
    "metadata": {},
    "name": "zoneCard_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_updateZoneMutation(\n  $input: UpdateZoneInput!\n) {\n  updateZone(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "47efbccca3d4f428d3cd14d5736e95f3";

export default node;
