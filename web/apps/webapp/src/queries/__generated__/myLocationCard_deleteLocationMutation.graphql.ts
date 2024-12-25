/**
 * @generated SignedSource<<99476c98eb31c9341c9a959d03db72a2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type myLocationCard_deleteLocationMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteLocationInput;
};
export type myLocationCard_deleteLocationMutation$data = {
  readonly deleteLocation: {
    readonly location: {
      readonly id: string;
    };
  } | null | undefined;
};
export type myLocationCard_deleteLocationMutation = {
  response: myLocationCard_deleteLocationMutation$data;
  variables: myLocationCard_deleteLocationMutation$variables;
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
    "name": "myLocationCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
    "name": "myLocationCard_deleteLocationMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
    "cacheID": "35a07d355d2ab53cd815dede74e823bc",
    "id": null,
    "metadata": {},
    "name": "myLocationCard_deleteLocationMutation",
    "operationKind": "mutation",
    "text": "mutation myLocationCard_deleteLocationMutation(\n  $input: DeleteLocationInput!\n) {\n  deleteLocation(input: $input) {\n    location {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7bd324e2ad2b5616bc17af85366dcd62";

export default node;
