/**
 * @generated SignedSource<<be70e755f8eca746d3c6b53ba053447e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type hostListingProductReadySubscription$variables = {
  locationId: string;
};
export type hostListingProductReadySubscription$data = {
  readonly listingProductReady: {
    readonly locationId: string;
    readonly product: {
      readonly id: string;
      readonly inactive: boolean;
    } | null | undefined;
  };
};
export type hostListingProductReadySubscription = {
  response: hostListingProductReadySubscription$data;
  variables: hostListingProductReadySubscription$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      }
    ],
    "concreteType": "HostListingProductReady",
    "kind": "LinkedField",
    "name": "listingProductReady",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "locationId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "HostListingProductReadyProduct",
        "kind": "LinkedField",
        "name": "product",
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
    "name": "hostListingProductReadySubscription",
    "selections": (v1/*:: as any*/),
    "type": "Subscription",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "hostListingProductReadySubscription",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "fac9d8503f96a4bd18ffe2b5f673f8c1",
    "id": null,
    "metadata": {},
    "name": "hostListingProductReadySubscription",
    "operationKind": "subscription",
    "text": "subscription hostListingProductReadySubscription(\n  $locationId: String!\n) {\n  listingProductReady(locationId: $locationId) {\n    locationId\n    product {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "82ebfd4a3e3487ae5fa08bb7837545a6";

export default node;
