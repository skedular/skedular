/**
 * @generated SignedSource<<c39e1a37349201c2c568db0603dba44a>>
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
          (v2/*: any*/)
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
    "cacheID": "cd366327aaed1194920afb1ef29a53ed",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_booking_Subscription",
    "operationKind": "subscription",
    "text": "subscription payMarketplaceBooking_booking_Subscription(\n  $bookingId: String!\n) {\n  booking(id: $bookingId) {\n    paymentStatus {\n      type\n      name\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "d41044375db258d7813bf99a4cd3ae9d";

export default node;
