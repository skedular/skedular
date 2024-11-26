/**
 * @generated SignedSource<<50643ed2ec7b327a30bb38995de6a7ab>>
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
  locationTagIds: ReadonlyArray<string>;
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
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
      }>;
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
      readonly locationTags: ReadonlyArray<{
        readonly id: string;
      }>;
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
],
v4 = {
  "alias": null,
  "args": null,
  "concreteType": "DeskDetails",
  "kind": "LinkedField",
  "name": "desk",
  "plural": false,
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
      "name": "deskTypes",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v3/*: any*/),
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
          (v4/*: any*/)
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
          (v4/*: any*/),
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
    "cacheID": "471175e40f40b1d812a73d2fa770a358",
    "id": null,
    "metadata": {},
    "name": "newDeskDialog_addDeskMutation",
    "operationKind": "mutation",
    "text": "mutation newDeskDialog_addDeskMutation(\n  $input: AddDeskInput!\n) {\n  addDesk(input: $input) {\n    desk {\n      id\n      name\n      locationTags {\n        id\n      }\n      deskTypes {\n        uniqueId\n      }\n      zones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4f3c66cd3f948f25b5bba75b5b99f8c5";

export default node;
