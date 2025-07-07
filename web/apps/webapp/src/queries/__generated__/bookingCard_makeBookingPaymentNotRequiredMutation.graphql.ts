/**
 * @generated SignedSource<<aa0863d09a4505a1b788d89018d0e318>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
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
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
    };
  };
};
export type bookingCard_makeBookingPaymentNotRequiredMutation$rawResponse = {
  readonly makeBookingPaymentNotRequired: {
    readonly booking: {
      readonly id: string;
      readonly paymentStatus: {
        readonly name: string;
        readonly type: PaymentStatus;
      };
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
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "adeef28e78efa4821bb537e4217a3ae8",
    "id": null,
    "metadata": {},
    "name": "bookingCard_makeBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_makeBookingPaymentNotRequiredMutation(\n  $input: MakeBookingPaymentNotRequiredInput!\n) {\n  makeBookingPaymentNotRequired(input: $input) {\n    booking {\n      id\n      paymentStatus {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f7d67c228b562307bfde0ee366e23940";

export default node;
