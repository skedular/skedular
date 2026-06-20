/**
 * @generated SignedSource<<b4524b75e04ff0133780d2304855f946>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type ConfirmBookingPaymentInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type payMarketplaceBooking_confirmBookingPaymentMutation$variables = {
  input: ConfirmBookingPaymentInput;
};
export type payMarketplaceBooking_confirmBookingPaymentMutation$data = {
  readonly confirmBookingPayment: {
    readonly booking: {
      readonly id: string;
      readonly marketplaceBooking: {
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
      } | null | undefined;
    } | null | undefined;
  };
};
export type payMarketplaceBooking_confirmBookingPaymentMutation$rawResponse = {
  readonly confirmBookingPayment: {
    readonly booking: {
      readonly id: string;
      readonly marketplaceBooking: {
        readonly id: string;
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
      } | null | undefined;
    } | null | undefined;
  };
};
export type payMarketplaceBooking_confirmBookingPaymentMutation = {
  rawResponse: payMarketplaceBooking_confirmBookingPaymentMutation$rawResponse;
  response: payMarketplaceBooking_confirmBookingPaymentMutation$data;
  variables: payMarketplaceBooking_confirmBookingPaymentMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "confirmBookingPayment",
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
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v3/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "confirmBookingPayment",
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
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceBookingDetails",
                "kind": "LinkedField",
                "name": "marketplaceBooking",
                "plural": false,
                "selections": [
                  (v3/*:: as any*/),
                  (v2/*:: as any*/)
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
    "cacheID": "dd2b57161ceb7900d546b9b2b0f390ac",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_confirmBookingPaymentMutation(\n  $input: ConfirmBookingPaymentInput!\n) {\n  confirmBookingPayment(input: $input) {\n    booking {\n      id\n      marketplaceBooking {\n        paymentStatus {\n          type\n          name\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "56b30a267ce410f19aa7fca55409b792";

export default node;
