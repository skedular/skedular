/**
 * @generated SignedSource<<7565bc97e917a1ad6759917f8016e782>>
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
export type editMarketplaceBooking_confirmBookingPaymentMutation$variables = {
  input: ConfirmBookingPaymentInput;
};
export type editMarketplaceBooking_confirmBookingPaymentMutation$data = {
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
export type editMarketplaceBooking_confirmBookingPaymentMutation$rawResponse = {
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
export type editMarketplaceBooking_confirmBookingPaymentMutation = {
  rawResponse: editMarketplaceBooking_confirmBookingPaymentMutation$rawResponse;
  response: editMarketplaceBooking_confirmBookingPaymentMutation$data;
  variables: editMarketplaceBooking_confirmBookingPaymentMutation$variables;
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
    "name": "editMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_confirmBookingPaymentMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "cd60d9c57ddb610700a5beade230d1f1",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_confirmBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_confirmBookingPaymentMutation(\n  $input: ConfirmBookingPaymentInput!\n) {\n  confirmBookingPayment(input: $input) {\n    booking {\n      id\n      marketplaceBooking {\n        id\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1b2b51e06084a33fac3f5b4531dd3414";

export default node;
