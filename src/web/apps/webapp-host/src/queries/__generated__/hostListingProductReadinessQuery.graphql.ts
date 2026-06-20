/**
 * @generated SignedSource<<b2c3dfcc18dc39baffb1e22a2a40ce6d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type hostListingProductReadinessQuery$variables = {
  locationId: string;
};
export type hostListingProductReadinessQuery$data = {
  readonly location: {
    readonly id: string;
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  } | null | undefined;
};
export type hostListingProductReadinessQuery = {
  response: hostListingProductReadinessQuery$data;
  variables: hostListingProductReadinessQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "locationId"
      }
    ],
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "location",
    "plural": false,
    "selections": [
      (v1/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "products",
        "plural": true,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "inactive",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "hostListingProductReadinessQuery",
    "selections": (v2/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "hostListingProductReadinessQuery",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "a1061dd3a3ebc249e17db6f4f0b589fa",
    "id": null,
    "metadata": {},
    "name": "hostListingProductReadinessQuery",
    "operationKind": "query",
    "text": "query hostListingProductReadinessQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    id\n    products {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ac52ff40e21445b8646ea376e5d5698a";

export default node;
