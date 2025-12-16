/**
 * @generated SignedSource<<e6142e6e18a5951a1db114eed3232d23>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type payMarketplaceBooking_booking_Subscription$variables = {
  bookingId: string;
};
export type payMarketplaceBooking_booking_Subscription$data = {
  readonly booking: {
    readonly bookingCheckoutSession: {
      readonly checkoutUrl: string;
    } | null | undefined;
    readonly invoiceUrl: string | null | undefined;
    readonly paymentExpiry: any;
    readonly paymentStatus: {
      readonly name: string;
      readonly type: PaymentStatus;
    };
  };
};
export type payMarketplaceBooking_booking_Subscription = {
  response: payMarketplaceBooking_booking_Subscription$data;
  variables: payMarketplaceBooking_booking_Subscription$variables;
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
  "name": "paymentExpiry",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
},
v4 = {
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
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "PaymentStatusDetails",
  "kind": "LinkedField",
  "name": "paymentStatus",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "name",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Subscription",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "bf774610a8a2c28fb343e29a26220246",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_booking_Subscription",
    "operationKind": "subscription",
    "text": "subscription payMarketplaceBooking_booking_Subscription(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    paymentExpiry\n    invoiceUrl\n    bookingCheckoutSession {\n      checkoutUrl\n    }\n    paymentStatus {\n      type\n      name\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "9d05dc1f9111fa5b247bea338cf0e974";

export default node;
