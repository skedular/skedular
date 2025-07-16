/**
 * @generated SignedSource<<c2922934a648fb4e93f83705b879e988>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
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
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
    };
  };
};
export type payMarketplaceBooking_confirmBookingPaymentMutation$rawResponse = {
  readonly confirmBookingPayment: {
    readonly booking: {
      readonly id: string;
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
    };
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
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
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "30258bf96f04e977b056197d2a4f3e93",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_confirmBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_confirmBookingPaymentMutation(\n  $input: ConfirmBookingPaymentInput!\n) {\n  confirmBookingPayment(input: $input) {\n    booking {\n      id\n      paymentStatus {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e07f255c2e87ff0bc0040a60b6016e6f";

export default node;
