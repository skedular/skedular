/**
 * @generated SignedSource<<f97c873011b9bf86ef53a44060a2f755>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type locationCard_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationInput;
};
export type locationCard_deleteLocationMutation$data = {
  readonly deleteLocation: {
    readonly location: {
      readonly id: string;
    };
  };
};
export type locationCard_deleteLocationMutation = {
  response: locationCard_deleteLocationMutation$data;
  variables: locationCard_deleteLocationMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationPayload",
        "kind": "LinkedField",
        "name": "deleteLocation",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v2/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "locationCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationPayload",
        "kind": "LinkedField",
        "name": "deleteLocation",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
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
    "cacheID": "6643e31c4bdeb84adb19e3ad90e4cd20",
    "id": null,
    "metadata": {},
    "name": "locationCard_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationCard_deleteLocationMutation(\n  $input: DeleteLocationInput!\n) {\n  deleteLocation(input: $input) {\n    location {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7791edd8164873f3bad9b4c429f89cd8";

export default node;
