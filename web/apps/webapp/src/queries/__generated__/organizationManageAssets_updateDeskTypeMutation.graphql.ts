/**
 * @generated SignedSource<<9b993f2694c257f40cebf738b34409fa>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateDeskTypeInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type organizationManageAssets_updateDeskTypeMutation$variables = {
  input: UpdateDeskTypeInput;
};
export type organizationManageAssets_updateDeskTypeMutation$data = {
  readonly updateDeskType: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type organizationManageAssets_updateDeskTypeMutation = {
  response: organizationManageAssets_updateDeskTypeMutation$data;
  variables: organizationManageAssets_updateDeskTypeMutation$variables;
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
    "name": "updateDeskType",
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
    "name": "organizationManageAssets_updateDeskTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationManageAssets_updateDeskTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2cb4771f7078980921dc4491443a3bdb",
    "id": null,
    "metadata": {},
    "name": "organizationManageAssets_updateDeskTypeMutation",
    "operationKind": "mutation",
    "text": "mutation organizationManageAssets_updateDeskTypeMutation(\n  $input: UpdateDeskTypeInput!\n) {\n  updateDeskType(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3a20d33a34903e1152918d40f8a4fedf";

export default node;
