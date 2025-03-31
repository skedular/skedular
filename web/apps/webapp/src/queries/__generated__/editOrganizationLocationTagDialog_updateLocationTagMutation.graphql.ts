/**
 * @generated SignedSource<<95643c6dd05eca46be2d82280d9fc006>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateLocationTagInput = {
  clientMutationId?: string | null | undefined;
  color?: string | null | undefined;
  description?: string | null | undefined;
  id: string;
  name: string;
};
export type editOrganizationLocationTagDialog_updateLocationTagMutation$variables = {
  input: UpdateLocationTagInput;
};
export type editOrganizationLocationTagDialog_updateLocationTagMutation$data = {
  readonly updateLocationTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationLocationTagDialog_updateLocationTagMutation$rawResponse = {
  readonly updateLocationTag: {
    readonly organizationTag: {
      readonly color: string | null | undefined;
      readonly description: string | null | undefined;
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type editOrganizationLocationTagDialog_updateLocationTagMutation = {
  rawResponse: editOrganizationLocationTagDialog_updateLocationTagMutation$rawResponse;
  response: editOrganizationLocationTagDialog_updateLocationTagMutation$data;
  variables: editOrganizationLocationTagDialog_updateLocationTagMutation$variables;
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
    "name": "updateLocationTag",
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
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
    "name": "editOrganizationLocationTagDialog_updateLocationTagMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editOrganizationLocationTagDialog_updateLocationTagMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "d183a3ce9b9e7eb8c973cfc43e4892ae",
    "id": null,
    "metadata": {},
    "name": "editOrganizationLocationTagDialog_updateLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation editOrganizationLocationTagDialog_updateLocationTagMutation(\n  $input: UpdateLocationTagInput!\n) {\n  updateLocationTag(input: $input) {\n    organizationTag {\n      id\n      name\n      description\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e534d3f3a5dcb304c85f2c4e43e18c60";

export default node;
