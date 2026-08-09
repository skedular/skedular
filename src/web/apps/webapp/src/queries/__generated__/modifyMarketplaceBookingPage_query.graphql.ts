/**
 * @generated SignedSource<<212e8c2114e3be1b1380810e040ee49b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type modifyMarketplaceBookingPage_query$variables = {
  bookingId: string;
};
export type modifyMarketplaceBookingPage_query$data = {
  readonly booking: {
    readonly bookingResources: ReadonlyArray<{
      readonly resource: {
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly entityFrameworkVersion: any;
    readonly from: any;
    readonly id: string;
    readonly involvedLocations: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
    readonly until: any;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"modifyMarketplaceBookingDialog_query">;
};
export type modifyMarketplaceBookingPage_query = {
  response: modifyMarketplaceBookingPage_query$data;
  variables: modifyMarketplaceBookingPage_query$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "bookingId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "entityFrameworkVersion",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "until",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "Booking_LocationDetails",
  "kind": "LinkedField",
  "name": "involvedLocations",
  "plural": true,
  "selections": [
    (v6/*:: as any*/)
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "ResourceDetails",
    "kind": "LinkedField",
    "name": "resource",
    "plural": false,
    "selections": [
      (v2/*:: as any*/),
      (v8/*:: as any*/)
    ],
    "storageKey": null
  }
],
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingResourceDetails",
  "kind": "LinkedField",
  "name": "bookingResources",
  "plural": true,
  "selections": (v9/*:: as any*/),
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "modifyMarketplaceBookingPage_query",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "bookingId",
            "variableName": "bookingId"
          }
        ],
        "kind": "FragmentSpread",
        "name": "modifyMarketplaceBookingDialog_query"
      },
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v7/*:: as any*/),
          (v10/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "modifyMarketplaceBookingPage_query",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingResourceSelectionDetails",
            "kind": "LinkedField",
            "name": "marketplaceBookingResourceSelection",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "canSelectResources",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maximumResourceCount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "availableResourceIds",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_LocationDetails",
                "kind": "LinkedField",
                "name": "eligibleLocations",
                "plural": true,
                "selections": [
                  (v6/*:: as any*/),
                  (v8/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingResourceDetails",
                "kind": "LinkedField",
                "name": "eligibleResources",
                "plural": true,
                "selections": (v9/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v2/*:: as any*/),
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v7/*:: as any*/),
          (v10/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "680153b47e0cd717a1f6a4c6bbf02183",
    "id": null,
    "metadata": {},
    "name": "modifyMarketplaceBookingPage_query",
    "operationKind": "query",
    "text": "query modifyMarketplaceBookingPage_query(\n  $bookingId: String!\n) {\n  ...modifyMarketplaceBookingDialog_query_378Z3H\n  booking(id: $bookingId) {\n    id\n    entityFrameworkVersion\n    from\n    until\n    involvedLocations {\n      uniqueId\n    }\n    bookingResources {\n      resource {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment modifyMarketplaceBookingDialog_query_378Z3H on Query {\n  booking(id: $bookingId) {\n    marketplaceBookingResourceSelection {\n      canSelectResources\n      maximumResourceCount\n      availableResourceIds\n      eligibleLocations {\n        uniqueId\n        name\n      }\n      eligibleResources {\n        resource {\n          id\n          name\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "e41749d871cd5bdd3d0681e10c48f15c";

export default node;
