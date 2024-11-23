/**
 * @generated SignedSource<<1565ee35591519789542f399e99b59d9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddOrganizationTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
  tagType: string;
};
export type newDeskTypeDialog_addDeskTypeMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddOrganizationTagInput;
};
export type newDeskTypeDialog_addDeskTypeMutation$data = {
  readonly addOrganizationTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newDeskTypeDialog_addDeskTypeMutation$rawResponse = {
  readonly addOrganizationTag: {
    readonly organizationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newDeskTypeDialog_addDeskTypeMutation = {
  rawResponse: newDeskTypeDialog_addDeskTypeMutation$rawResponse;
  response: newDeskTypeDialog_addDeskTypeMutation$data;
  variables: newDeskTypeDialog_addDeskTypeMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "newDeskTypeDialog_addDeskTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addOrganizationTag",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "newDeskTypeDialog_addDeskTypeMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationTagPayload",
        "kind": "LinkedField",
        "name": "addOrganizationTag",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "organizationTag",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "OrganizationTagDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "7992137a36e905780d9cbbea06793f0e",
    "id": null,
    "metadata": {},
    "name": "newDeskTypeDialog_addDeskTypeMutation",
    "operationKind": "mutation",
    "text": "mutation newDeskTypeDialog_addDeskTypeMutation(\n  $input: AddOrganizationTagInput!\n) {\n  addOrganizationTag(input: $input) {\n    organizationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "89df09bcd8aef788e833d4467e20b226";

export default node;
