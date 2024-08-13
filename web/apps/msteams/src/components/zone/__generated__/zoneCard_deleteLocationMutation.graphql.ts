/**
 * @generated SignedSource<<18fbeea6862faa38fed970e05d4d1eeb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type DeleteLocationTagInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type zoneCard_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationTagInput;
};
export type zoneCard_deleteLocationMutation$data = {
  readonly deleteLocationTag: {
    readonly locationTag: {
      readonly id: string;
    };
  } | null | undefined;
};
export type zoneCard_deleteLocationMutation = {
  response: zoneCard_deleteLocationMutation$data;
  variables: zoneCard_deleteLocationMutation$variables;
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
    "name": "zoneCard_deleteLocationMutation",
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
    "name": "zoneCard_deleteLocationMutation",
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
    "cacheID": "016e848025e0b5528196bc7ea831322d",
    "id": null,
    "metadata": {},
    "name": "zoneCard_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation zoneCard_deleteLocationMutation(\n  $input: DeleteLocationTagInput!\n) {\n  deleteLocationTag(input: $input) {\n    locationTag {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "488c12c74471675e9b05e851ea92b5ad";

export default node;
