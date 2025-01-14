/**
 * @generated SignedSource<<9e13030258a5274ea37419e74ad59438>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddDeskInput = {
  clientMutationId?: string | null | undefined;
  customTagIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  locationId: string;
  name: string;
  zoneIds: ReadonlyArray<string>;
};
export type addDeskDialog_addDeskMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddDeskInput;
};
export type addDeskDialog_addDeskMutation$data = {
  readonly addDesk: {
    readonly desk: {
      readonly customTags: ReadonlyArray<{
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
export type addDeskDialog_addDeskMutation$rawResponse = {
  readonly addDesk: {
    readonly desk: {
      readonly customTags: ReadonlyArray<{
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
export type addDeskDialog_addDeskMutation = {
  rawResponse: addDeskDialog_addDeskMutation$rawResponse;
  response: addDeskDialog_addDeskMutation$data;
  variables: addDeskDialog_addDeskMutation$variables;
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
      "name": "customTags",
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
    "name": "addDeskDialog_addDeskMutation",
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
    "name": "addDeskDialog_addDeskMutation",
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
    "cacheID": "a8cc5667eea5e7034334fb78055d0138",
    "id": null,
    "metadata": {},
    "name": "addDeskDialog_addDeskMutation",
    "operationKind": "mutation",
    "text": "mutation addDeskDialog_addDeskMutation(\n  $input: AddDeskInput!\n) {\n  addDesk(input: $input) {\n    desk {\n      id\n      name\n      customTags {\n        uniqueId\n      }\n      zones {\n        uniqueId\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "117309c7c73de6742e8d83a529e5850f";

export default node;
