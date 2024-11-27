/**
 * @generated SignedSource<<05ba8452332746885600a1a9544830bc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddDeskInput = {
  clientMutationId?: string | null | undefined;
  deskTypeIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  locationId: string;
  name: string;
  zoneIds: ReadonlyArray<string>;
};
export type newDeskDialog_addDeskMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddDeskInput;
};
export type newDeskDialog_addDeskMutation$data = {
  readonly addDesk: {
    readonly desk: {
      readonly deskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type newDeskDialog_addDeskMutation$rawResponse = {
  readonly addDesk: {
    readonly desk: {
      readonly deskTypes: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
      readonly id: string;
      readonly name: string;
      readonly zones: ReadonlyArray<{
        readonly uniqueId: string;
      }>;
    };
  } | null | undefined;
};
export type newDeskDialog_addDeskMutation = {
  rawResponse: newDeskDialog_addDeskMutation$rawResponse;
  response: newDeskDialog_addDeskMutation$data;
  variables: newDeskDialog_addDeskMutation$variables;
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
  "name": "desk",
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
    "name": "newDeskDialog_addDeskMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DeskPayload",
        "kind": "LinkedField",
        "name": "addDesk",
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
    "name": "newDeskDialog_addDeskMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "DeskPayload",
        "kind": "LinkedField",
        "name": "addDesk",
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
            "name": "desk",
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
    "cacheID": "bcdf2f655a1b6c2e1c30d74cf391a224",
    "id": null,
    "metadata": {},
    "name": "newDeskDialog_addDeskMutation",
    "operationKind": "mutation",
    "text": "mutation newDeskDialog_addDeskMutation(\n  $input: AddDeskInput!\n) {\n  addDesk(input: $input) {\n    desk {\n      id\n      name\n      deskTypes {\n        uniqueId\n      }\n      zones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2a66b8f87bbd0f51dafd6f01f5ddb90a";

export default node;
