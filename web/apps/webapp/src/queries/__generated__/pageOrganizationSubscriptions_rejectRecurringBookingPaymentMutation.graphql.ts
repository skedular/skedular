/**
 * @generated SignedSource<<24b3b7345af6103c68a614bc2727371c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type RejectRecurringBookingPaymentInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$variables = {
  input: RejectRecurringBookingPaymentInput;
};
export type pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$data = {
  readonly rejectRecurringBookingPayment: {
    readonly recurringBooking: {
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
export type pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$rawResponse = {
  readonly rejectRecurringBookingPayment: {
    readonly recurringBooking: {
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
export type pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation = {
  rawResponse: pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$rawResponse;
  response: pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$data;
  variables: pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation$variables;
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
    "concreteType": "RecurringBookingPayload",
    "kind": "LinkedField",
    "name": "rejectRecurringBookingPayment",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "RecurringBookingDetails",
        "kind": "LinkedField",
        "name": "recurringBooking",
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
    "name": "pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "8d6b3c5a617a313ce13ec7be92601f99",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptions_rejectRecurringBookingPaymentMutation(\n  $input: RejectRecurringBookingPaymentInput!\n) {\n  rejectRecurringBookingPayment(input: $input) {\n    recurringBooking {\n      id\n      marketplaceBooking {\n        id\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d960adb8f7df08744b93cc29ea85d71c";

export default node;
