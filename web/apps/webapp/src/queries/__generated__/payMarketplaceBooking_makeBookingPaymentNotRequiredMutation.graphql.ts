/**
 * @generated SignedSource<<3a9a6bdd7313c39aa6701c7f7edf3a09>>
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
export type payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$variables = {
  input: MakeBookingPaymentNotRequiredInput;
};
export type payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$data = {
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
export type payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$rawResponse = {
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
export type payMarketplaceBooking_makeBookingPaymentNotRequiredMutation = {
  rawResponse: payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$rawResponse;
  response: payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$data;
  variables: payMarketplaceBooking_makeBookingPaymentNotRequiredMutation$variables;
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
    "name": "payMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "payMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "430e5d218572a51c32ff5280ef965617",
    "id": null,
    "metadata": {},
    "name": "payMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation payMarketplaceBooking_makeBookingPaymentNotRequiredMutation(\n  $input: MakeBookingPaymentNotRequiredInput!\n) {\n  makeBookingPaymentNotRequired(input: $input) {\n    booking {\n      id\n      paymentStatus {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1b9b971e8e2b0d29e36c52589769152c";

export default node;
