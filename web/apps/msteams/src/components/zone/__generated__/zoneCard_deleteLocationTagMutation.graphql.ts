/**
 * @generated SignedSource<<48a1fd85e0323dcb11bb3da77c23c143>>
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
export type zoneCard_deleteLocationTagMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationTagInput;
};
export type zoneCard_deleteLocationTagMutation$data = {
  readonly deleteLocationTag: {
    readonly locationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type zoneCard_deleteLocationTagMutation = {
  response: zoneCard_deleteLocationTagMutation$data;
  variables: zoneCard_deleteLocationTagMutation$variables;
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
    "name": "zoneCard_deleteLocationTagMutation",
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
    "name": "zoneCard_deleteLocationTagMutation",
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
    "cacheID": "f5da1421376de0aa1f208c51f51d998a",
    "id": null,
    "metadata": {},
    "name": "zoneCard_deleteLocationTagMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_deleteLocationTagMutation(\n  $input: DeleteLocationTagInput!\n) {\n  deleteLocationTag(input: $input) {\n    locationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "72dc7098aa5686bfd6bb69fba8eee1c9";

export default node;
