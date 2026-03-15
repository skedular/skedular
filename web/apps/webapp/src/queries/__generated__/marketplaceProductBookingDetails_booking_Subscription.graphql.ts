/**
 * @generated SignedSource<<b9386db225fc8406c69bba173a552cc6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type marketplaceProductBookingDetails_booking_Subscription$variables = {
  bookingId: string;
};
export type marketplaceProductBookingDetails_booking_Subscription$data = {
  readonly booking: {
    readonly marketplaceBooking: {
      readonly bookingCheckoutSession: {
        readonly checkoutUrl: string;
      } | null | undefined;
      readonly id: string;
      readonly invoiceUrl: string | null | undefined;
      readonly isPaymentRequired: boolean;
      readonly paymentExpiry: any;
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
    } | null | undefined;
  };
};
export type marketplaceProductBookingDetails_booking_Subscription = {
  response: marketplaceProductBookingDetails_booking_Subscription$data;
  variables: marketplaceProductBookingDetails_booking_Subscription$variables;
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
  "concreteType": "MarketplaceBookingDetails",
  "kind": "LinkedField",
  "name": "marketplaceBooking",
  "plural": false,
  "selections": [
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "invoiceUrl",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isPaymentRequired",
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
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v3/*: any*/)
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
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v2/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "bafd6b890ce623e6080cf88567f3a324",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductBookingDetails_booking_Subscription",
    "operationKind": "subscription",
    "text": "subscription marketplaceProductBookingDetails_booking_Subscription(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    marketplaceBooking {\n      id\n      invoiceUrl\n      isPaymentRequired\n      paymentExpiry\n      bookingCheckoutSession {\n        checkoutUrl\n      }\n      paymentStatus {\n        type\n        name\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "dceab27bff473494cb4788a06a90b8ec";

export default node;
