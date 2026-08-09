/**
 * @generated SignedSource<<18714b714c953bc1ca92dc8068b80a3e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type modifyMarketplaceBookingDialog_booking_refetchableFragment$variables = {
  bookingId: string;
  from?: any | null | undefined;
  locationId?: string | null | undefined;
  until?: any | null | undefined;
};
export type modifyMarketplaceBookingDialog_booking_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"modifyMarketplaceBookingDialog_query">;
};
export type modifyMarketplaceBookingDialog_booking_refetchableFragment = {
  response: modifyMarketplaceBookingDialog_booking_refetchableFragment$data;
  variables: modifyMarketplaceBookingDialog_booking_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "from"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "until"
  }
],
v1 = {
  "kind": "Variable",
  "name": "from",
  "variableName": "from"
},
v2 = {
  "kind": "Variable",
  "name": "locationId",
  "variableName": "locationId"
},
v3 = {
  "kind": "Variable",
  "name": "until",
  "variableName": "until"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
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
    "name": "modifyMarketplaceBookingDialog_booking_refetchableFragment",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "bookingId",
            "variableName": "bookingId"
          },
          (v1/*:: as any*/),
          (v2/*:: as any*/),
          (v3/*:: as any*/)
        ],
        "kind": "FragmentSpread",
        "name": "modifyMarketplaceBookingDialog_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "modifyMarketplaceBookingDialog_booking_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "bookingId"
          }
        ],
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": [
              (v1/*:: as any*/),
              (v2/*:: as any*/),
              (v3/*:: as any*/)
            ],
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
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "uniqueId",
                    "storageKey": null
                  },
                  (v4/*:: as any*/)
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
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "resource",
                    "plural": false,
                    "selections": [
                      (v5/*:: as any*/),
                      (v4/*:: as any*/)
                    ],
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v5/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "d7cec9b2c53ab8905c8951238a493c22",
    "id": null,
    "metadata": {},
    "name": "modifyMarketplaceBookingDialog_booking_refetchableFragment",
    "operationKind": "query",
    "text": "query modifyMarketplaceBookingDialog_booking_refetchableFragment(\n  $bookingId: String!\n  $from: DateTime\n  $locationId: String\n  $until: DateTime\n) {\n  ...modifyMarketplaceBookingDialog_query_2yo5QN\n}\n\nfragment modifyMarketplaceBookingDialog_query_2yo5QN on Query {\n  booking(id: $bookingId) {\n    marketplaceBookingResourceSelection(from: $from, until: $until, locationId: $locationId) {\n      canSelectResources\n      maximumResourceCount\n      availableResourceIds\n      eligibleLocations {\n        uniqueId\n        name\n      }\n      eligibleResources {\n        resource {\n          id\n          name\n        }\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "51b0fcb8120af9d9886747f1a2c12000";

export default node;
