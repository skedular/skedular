/**
 * @generated SignedSource<<639265d2c36502146a9271ba5d4d4289>>
 * @lightSyntaxTransform
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
export type editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$variables = {
  input: MakeBookingPaymentNotRequiredInput;
};
export type editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$data = {
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
export type editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$rawResponse = {
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
export type editMarketplaceBooking_makeBookingPaymentNotRequiredMutation = {
  rawResponse: editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$rawResponse;
  response: editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$data;
  variables: editMarketplaceBooking_makeBookingPaymentNotRequiredMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v1/*:: as any*/),
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
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "5721047bdbfddd00601b2c905776a8db",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_makeBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_makeBookingPaymentNotRequiredMutation(\n  $input: MakeBookingPaymentNotRequiredInput!\n) {\n  makeBookingPaymentNotRequired(input: $input) {\n    booking {\n      id\n      marketplaceBooking {\n        id\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "38eb3592c45cc2abc620d59e6767b4c5";

export default node;
