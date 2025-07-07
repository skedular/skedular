/**
 * @generated SignedSource<<e19fe5ab51ef94f1e4f8654ea03190ed>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type payMarketplaceBooking_booking_refetchableFragment$variables = {
  bookingId: string;
};
export type payMarketplaceBooking_booking_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"payMarketplaceBooking_booking_query">;
};
export type payMarketplaceBooking_booking_refetchableFragment = {
  response: payMarketplaceBooking_booking_refetchableFragment$data;
  variables: payMarketplaceBooking_booking_refetchableFragment$variables;
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
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  (v2/*: any*/),
  (v3/*: any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v6 = [
  (v2/*: any*/),
  (v3/*: any*/),
  (v5/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_booking_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "payMarketplaceBooking_booking_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_booking_refetchableFragment",
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
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "from",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "until",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "notes",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v1/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "givenName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "middleName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "familyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "photoUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_OrganizationDetails",
            "kind": "LinkedField",
            "name": "involvedOrganizations",
            "plural": true,
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
            "selections": (v4/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingResourceDetails",
            "kind": "LinkedField",
            "name": "resources",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_OrganizationCustomTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v6/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_OrganizationZoneDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v6/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalAmountToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PaymentMethodTypeDetails",
            "kind": "LinkedField",
            "name": "paymentMethod",
            "plural": false,
            "selections": (v1/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCheckoutSessionDetails",
            "kind": "LinkedField",
            "name": "bookingCheckoutSession",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "checkoutUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "paymentExpiry",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LineItemDetails",
            "kind": "LinkedField",
            "name": "lineItems",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "quantity",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_ProductVersionDetails",
                "kind": "LinkedField",
                "name": "productVersion",
                "plural": false,
                "selections": [
                  (v2/*: any*/),
                  (v3/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "priceToDisplay",
                    "storageKey": null
                  }
                ],
                "storageKey": null
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
    "cacheID": "038dace5556fc23d93443bc2832d0631",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_booking_refetchableFragment",
    "operationKind": "query",
    "text": "query payMarketplaceBooking_booking_refetchableFragment(\n  $bookingId: String!\n) {\n  ...payMarketplaceBooking_booking_query\n}\n\nfragment payMarketplaceBooking_booking_query on Query {\n  booking(id: $bookingId) {\n    id\n    from\n    until\n    notes\n    type {\n      type\n    }\n    involvedCustomers {\n      uniqueId\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n    involvedOrganizations {\n      uniqueId\n      name\n    }\n    involvedLocations {\n      uniqueId\n      name\n    }\n    involvedTeams {\n      uniqueId\n      name\n    }\n    resources {\n      uniqueId\n      name\n      color\n      customTags {\n        uniqueId\n        name\n        color\n      }\n      zones {\n        uniqueId\n        name\n        color\n      }\n    }\n    totalAmountToDisplay\n    paymentMethod {\n      type\n    }\n    bookingCheckoutSession {\n      checkoutUrl\n    }\n    paymentExpiry\n    lineItems {\n      quantity\n      productVersion {\n        uniqueId\n        name\n        priceToDisplay\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "56b2bfacbbcd487bc530dd936c2b582f";

export default node;
