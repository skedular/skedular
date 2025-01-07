/**
 * @generated SignedSource<<f2f9a973d9524371ddc6ba4a9b88aa99>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateZoneInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type organizationManageAssets_updateZoneMutation$variables = {
  input: UpdateZoneInput;
};
export type organizationManageAssets_updateZoneMutation$data = {
  readonly updateZone: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type organizationManageAssets_updateZoneMutation = {
  response: organizationManageAssets_updateZoneMutation$data;
  variables: organizationManageAssets_updateZoneMutation$variables;
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
    "name": "organizationManageAssets_updateZoneMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationManageAssets_updateZoneMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "04cb6a99655cf05b602b02346ff7fafb",
    "id": null,
    "metadata": {},
    "name": "organizationManageAssets_updateZoneMutation",
    "operationKind": "mutation",
    "text": "mutation organizationManageAssets_updateZoneMutation(\n  $input: UpdateZoneInput!\n) {\n  updateZone(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9c6c09efcc0a1e767cc13758f9e19895";

export default node;
