/**
 * @generated SignedSource<<aea9c85130a432cb6f01149207d65fe0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddLocationTagInput = {
  clientMutationId?: string | null | undefined;
  description?: string | null | undefined;
  id?: string | null | undefined;
  locationId: string;
  name: string;
  tagType: string;
};
export type newOldZoneDialog_addZoneMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddLocationTagInput;
};
export type newOldZoneDialog_addZoneMutation$data = {
  readonly addLocationTag: {
    readonly locationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newOldZoneDialog_addZoneMutation$rawResponse = {
  readonly addLocationTag: {
    readonly locationTag: {
      readonly id: string;
      readonly name: string;
    };
  } | null | undefined;
};
export type newOldZoneDialog_addZoneMutation = {
  rawResponse: newOldZoneDialog_addZoneMutation$rawResponse;
  response: newOldZoneDialog_addZoneMutation$data;
  variables: newOldZoneDialog_addZoneMutation$variables;
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
  "concreteType": "LocationTagDetails",
  "kind": "LinkedField",
  "name": "locationTag",
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
    "name": "newOldZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationTagPayload",
        "kind": "LinkedField",
        "name": "addLocationTag",
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
    "name": "newOldZoneDialog_addZoneMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationTagPayload",
        "kind": "LinkedField",
        "name": "addLocationTag",
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
            "name": "locationTag",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "LocationTagDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "570d82abfe20bb2fc36a4860d7ccc1c4",
    "id": null,
    "metadata": {},
    "name": "newOldZoneDialog_addZoneMutation",
    "operationKind": "mutation",
    "text": "mutation newOldZoneDialog_addZoneMutation(\n  $input: AddLocationTagInput!\n) {\n  addLocationTag(input: $input) {\n    locationTag {\n      id\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "616027a3129826dc411d57bbdb863b48";

export default node;
