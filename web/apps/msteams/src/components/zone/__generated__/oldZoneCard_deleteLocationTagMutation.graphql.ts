/**
 * @generated SignedSource<<91c653c8b4dbd0571f798eedd4916648>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteLocationTagInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type oldZoneCard_deleteLocationTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationTagInput;
};
export type oldZoneCard_deleteLocationTagMutation$data = {
  readonly deleteLocationTag: {
    readonly locationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type oldZoneCard_deleteLocationTagMutation = {
  response: oldZoneCard_deleteLocationTagMutation$data;
  variables: oldZoneCard_deleteLocationTagMutation$variables;
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "oldZoneCard_deleteLocationTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationTagPayload",
        "kind": "LinkedField",
        "name": "deleteLocationTag",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationTagDetails",
            "kind": "LinkedField",
            "name": "locationTag",
            "plural": false,
            "selections": [
              (v2/*: any*/)
            ],
            "storageKey": null
          }
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
    "name": "oldZoneCard_deleteLocationTagMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationTagPayload",
        "kind": "LinkedField",
        "name": "deleteLocationTag",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationTagDetails",
            "kind": "LinkedField",
            "name": "locationTag",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "b787087c1e7c176bd0e2165042601941",
    "id": null,
    "metadata": {},
    "name": "oldZoneCard_deleteLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation oldZoneCard_deleteLocationTagMutation(\n  $input: DeleteLocationTagInput!\n) {\n  deleteLocationTag(input: $input) {\n    locationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2e1b8f09259e7dd9039f031872f6c093";

export default node;
