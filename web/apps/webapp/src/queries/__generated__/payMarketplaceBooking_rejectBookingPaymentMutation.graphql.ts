/**
 * @generated SignedSource<<299a341fdfe2eaea895097de571a3982>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type RejectBookingPaymentInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type payMarketplaceBooking_rejectBookingPaymentMutation$variables = {
  input: RejectBookingPaymentInput;
};
export type payMarketplaceBooking_rejectBookingPaymentMutation$data = {
  readonly rejectBookingPayment: {
    readonly booking: {
      readonly id: string;
      readonly marketplaceBooking: {
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
      } | null | undefined;
    };
  };
};
export type payMarketplaceBooking_rejectBookingPaymentMutation$rawResponse = {
  readonly rejectBookingPayment: {
    readonly booking: {
      readonly id: string;
      readonly marketplaceBooking: {
        readonly id: string;
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
      } | null | undefined;
    };
  };
};
export type payMarketplaceBooking_rejectBookingPaymentMutation = {
  rawResponse: payMarketplaceBooking_rejectBookingPaymentMutation$rawResponse;
  response: payMarketplaceBooking_rejectBookingPaymentMutation$data;
  variables: payMarketplaceBooking_rejectBookingPaymentMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
},
v3 = {
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
    "name": "payMarketplaceBooking_rejectBookingPaymentMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "rejectBookingPayment",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v3/*: any*/)
                ],
                "storageKey": null
              }
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
    "name": "payMarketplaceBooking_rejectBookingPaymentMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "rejectBookingPayment",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDetails",
            "kind": "LinkedField",
            "name": "booking",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v3/*: any*/),
                  (v2/*: any*/)
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
    "cacheID": "f5b4431e64a25e095f8128dc9f0bc86a",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_rejectBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_rejectBookingPaymentMutation(\n  $input: RejectBookingPaymentInput!\n) {\n  rejectBookingPayment(input: $input) {\n    booking {\n      id\n      marketplaceBooking {\n        paymentStatus {\n          type\n          name\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5394cc56d22b91a13e6f5b414504f733";

export default node;
