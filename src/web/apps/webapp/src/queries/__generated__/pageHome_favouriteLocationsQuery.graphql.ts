/**
 * @generated SignedSource<<b2dd498117cf9b42521c47e2d2b642ea>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageHome_favouriteLocationsQuery$variables = Record<PropertyKey, never>;
export type pageHome_favouriteLocationsQuery$data = {
  readonly me: {
    readonly favouriteLocations: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type pageHome_favouriteLocationsQuery = {
  response: pageHome_favouriteLocationsQuery$data;
  variables: pageHome_favouriteLocationsQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationDetails",
  "kind": "LinkedField",
  "name": "favouriteLocations",
  "plural": true,
  "selections": [
    (v0/*:: as any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageHome_favouriteLocationsQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageHome_favouriteLocationsQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
          (v0/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "cf2be60dc8db6a433814f7ba367f7f8a",
    "id": null,
    "metadata": {},
    "name": "pageHome_favouriteLocationsQuery",
    "operationKind": "query",
    "text": "query pageHome_favouriteLocationsQuery {\n  me {\n    favouriteLocations {\n      id\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "86389f23c8e29d929ca1117da622ce86";

export default node;
