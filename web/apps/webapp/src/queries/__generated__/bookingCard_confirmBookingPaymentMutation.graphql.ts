/**
 * @generated SignedSource<<602c429bec0580fee347963e43481090>>
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
export type bookingCard_confirmBookingPaymentMutation$variables = {
  input: ConfirmBookingPaymentInput;
};
export type bookingCard_confirmBookingPaymentMutation$data = {
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
export type bookingCard_confirmBookingPaymentMutation$rawResponse = {
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
export type bookingCard_confirmBookingPaymentMutation = {
  rawResponse: bookingCard_confirmBookingPaymentMutation$rawResponse;
  response: bookingCard_confirmBookingPaymentMutation$data;
  variables: bookingCard_confirmBookingPaymentMutation$variables;
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
    "name": "bookingCard_confirmBookingPaymentMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingCard_confirmBookingPaymentMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "0df26c8c05cb04a11a4ef4eabdc44f9f",
    "id": null,
    "metadata": {},
    "name": "bookingCard_confirmBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_confirmBookingPaymentMutation(\n  $input: ConfirmBookingPaymentInput!\n) {\n  confirmBookingPayment(input: $input) {\n    booking {\n      id\n      paymentStatus {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "763e350eb5f31f335b81a2775b64c2fc";

export default node;
