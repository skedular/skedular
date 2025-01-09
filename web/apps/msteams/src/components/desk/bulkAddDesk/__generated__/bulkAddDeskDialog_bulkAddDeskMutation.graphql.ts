/**
 * @generated SignedSource<<e39bfc3e211fc8d899f46a208fc0f4f3>>
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
  deskTypeIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  locationId: string;
  namePrefix?: string | null | undefined;
  requireBookingApproval: boolean;
  zoneIds: ReadonlyArray<string>;
};
export type bulkAddDeskDialog_bulkAddDeskMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: BulkAddDeskInput;
};
export type bulkAddDeskDialog_bulkAddDeskMutation$data = {
  readonly bulkAddDesk: {
    readonly desks: ReadonlyArray<{
      readonly deskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    }>;
  } | null | undefined;
};
export type bulkAddDeskDialog_bulkAddDeskMutation$rawResponse = {
  readonly bulkAddDesk: {
    readonly desks: ReadonlyArray<{
      readonly deskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    }>;
  } | null | undefined;
};
export type bulkAddDeskDialog_bulkAddDeskMutation = {
  rawResponse: bulkAddDeskDialog_bulkAddDeskMutation$rawResponse;
  response: bulkAddDeskDialog_bulkAddDeskMutation$data;
  variables: bulkAddDeskDialog_bulkAddDeskMutation$variables;
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
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
],
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "DeskDetails",
  "kind": "LinkedField",
  "name": "desks",
  "plural": true,
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
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "deskTypes",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v2/*: any*/),
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
    "name": "bulkAddDeskDialog_bulkAddDeskMutation",
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
    "name": "bulkAddDeskDialog_bulkAddDeskMutation",
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
    "cacheID": "29896e954f61b58dac412342daf104db",
    "id": null,
    "metadata": {},
    "name": "bulkAddDeskDialog_bulkAddDeskMutation",
    "operationKind": "mutation",
    "text": "mutation bulkAddDeskDialog_bulkAddDeskMutation(\n  $input: BulkAddDeskInput!\n) {\n  bulkAddDesk(input: $input) {\n    desks {\n      id\n      name\n      deskTypes {\n        uniqueId\n      }\n      zones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "21740ea8c13d30d5f6518982f9954a09";

export default node;
