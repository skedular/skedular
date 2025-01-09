/**
 * @generated SignedSource<<3ceabbdf5c923cd2d69892d9cfe34edd>>
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
export type editOrganizationDeskTypeDialog_updateDeskTypeMutation$variables = {
  input: UpdateDeskTypeInput;
};
export type editOrganizationDeskTypeDialog_updateDeskTypeMutation$data = {
  readonly updateDeskType: {
    readonly organizationTag: {
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationDeskTypeDialog_updateDeskTypeMutation$rawResponse = {
  readonly updateDeskType: {
    readonly organizationTag: {
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationDeskTypeDialog_updateDeskTypeMutation = {
  rawResponse: editOrganizationDeskTypeDialog_updateDeskTypeMutation$rawResponse;
  response: editOrganizationDeskTypeDialog_updateDeskTypeMutation$data;
  variables: editOrganizationDeskTypeDialog_updateDeskTypeMutation$variables;
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "description",
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
    "name": "editOrganizationDeskTypeDialog_updateDeskTypeMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationDeskTypeDialog_updateDeskTypeMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9a2ee35657a6848fa48e2554dd70adca",
    "id": null,
    "metadata": {},
    "name": "editOrganizationDeskTypeDialog_updateDeskTypeMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationDeskTypeDialog_updateDeskTypeMutation(\n  $input: UpdateDeskTypeInput!\n) {\n  updateDeskType(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5412f64488bf4781832197039f0f561d";

export default node;
