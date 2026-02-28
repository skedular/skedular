/**
 * @generated SignedSource<<bb697276ef6ef746409f62d7e4adac7d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type MakeBookingPaymentNotRequiredInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type bookingCard_makeBookingPaymentNotRequiredMutation$variables = {
  input: MakeBookingPaymentNotRequiredInput;
};
export type bookingCard_makeBookingPaymentNotRequiredMutation$data = {
  readonly makeBookingPaymentNotRequired: {
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
export type bookingCard_makeBookingPaymentNotRequiredMutation$rawResponse = {
  readonly makeBookingPaymentNotRequired: {
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
export type bookingCard_makeBookingPaymentNotRequiredMutation = {
  rawResponse: bookingCard_makeBookingPaymentNotRequiredMutation$rawResponse;
  response: bookingCard_makeBookingPaymentNotRequiredMutation$data;
  variables: bookingCard_makeBookingPaymentNotRequiredMutation$variables;
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
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "makeBookingPaymentNotRequired",
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
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "makeBookingPaymentNotRequired",
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
    "cacheID": "52cf85bb9e6a85bcb6565503bcae1edc",
    "id": null,
    "metadata": {},
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_makeBookingPaymentNotRequiredMutation(\n  $input: MakeBookingPaymentNotRequiredInput!\n) {\n  makeBookingPaymentNotRequired(input: $input) {\n    booking {\n      id\n      marketplaceBooking {\n        paymentStatus {\n          type\n          name\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9af7a896ec1292d870ab17992f24eac2";

export default node;
