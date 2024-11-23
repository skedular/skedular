/**
 * @generated SignedSource<<1f177fdcaa570cd12f963f3cf4024fec>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BulkAddDeskInput = {
  clientMutationId?: string | null | undefined;
  count: number;
  deactivated: boolean;
  id?: string | null | undefined;
  locationId: string;
  locationTagIds: ReadonlyArray<string>;
  namePrefix?: string | null | undefined;
  organizationTagIds: ReadonlyArray<string>;
  requireBookingApproval: boolean;
};
export type bulkNewDeskDialog_bulkAddDeskMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: BulkAddDeskInput;
};
export type bulkNewDeskDialog_bulkAddDeskMutation$data = {
  readonly bulkAddDesk: {
    readonly desks: ReadonlyArray<{
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly name: string;
      readonly organizationTags: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    }>;
  } | null | undefined;
};
export type bulkNewDeskDialog_bulkAddDeskMutation$rawResponse = {
  readonly bulkAddDesk: {
    readonly desks: ReadonlyArray<{
      readonly id: string;
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly name: string;
      readonly organizationTags: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    }>;
  } | null | undefined;
};
export type bulkNewDeskDialog_bulkAddDeskMutation = {
  rawResponse: bulkNewDeskDialog_bulkAddDeskMutation$rawResponse;
  response: bulkNewDeskDialog_bulkAddDeskMutation$data;
  variables: bulkNewDeskDialog_bulkAddDeskMutation$variables;
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
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "DeskDetails",
  "kind": "LinkedField",
  "name": "desks",
  "plural": true,
  "selections": [
    (v2/*: any*/),
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
      "concreteType": "LocationTagDetails",
      "kind": "LinkedField",
      "name": "locationTags",
      "plural": true,
      "selections": [
        (v2/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "organizationTags",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "uniqueId",
          "storageKey": null
        }
      ],
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
    "name": "bulkNewDeskDialog_bulkAddDeskMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BulkDeskPayload",
        "kind": "LinkedField",
        "name": "bulkAddDesk",
        "plural": false,
        "selections": [
          (v3/*: any*/)
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
    "name": "bulkNewDeskDialog_bulkAddDeskMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BulkDeskPayload",
        "kind": "LinkedField",
        "name": "bulkAddDesk",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "desks",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "DeskDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "0966897abab35b709472648ae9ee62a5",
    "id": null,
    "metadata": {},
    "name": "bulkNewDeskDialog_bulkAddDeskMutation",
    "operationKind": "mutation",
    "text": "mutation bulkNewDeskDialog_bulkAddDeskMutation(\n  $input: BulkAddDeskInput!\n) {\n  bulkAddDesk(input: $input) {\n    desks {\n      id\n      name\n      locationTags {\n        id\n      }\n      organizationTags {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "017168d203808d1b8ac710c9a8328cb8";

export default node;
