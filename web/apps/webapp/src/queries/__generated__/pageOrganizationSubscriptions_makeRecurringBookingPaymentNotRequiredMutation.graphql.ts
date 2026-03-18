/**
 * @generated SignedSource<<a38bf1e6e369d650bb5d81c6094f6c22>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
export type MakeRecurringBookingPaymentNotRequiredInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$variables = {
  input: MakeRecurringBookingPaymentNotRequiredInput;
};
export type pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$data = {
  readonly makeRecurringBookingPaymentNotRequired: {
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
export type pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$rawResponse = {
  readonly makeRecurringBookingPaymentNotRequired: {
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
export type pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation = {
  rawResponse: pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$rawResponse;
  response: pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$data;
  variables: pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation$variables;
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
    "name": "makeRecurringBookingPaymentNotRequired",
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingDetails",
            "kind": "LinkedField",
            "name": "marketplaceBooking",
            "plural": false,
            "selections": [
              (v1/*: any*/),
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "e4e935f09917525a03cc409ad7a4011b",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptions_makeRecurringBookingPaymentNotRequiredMutation(\n  $input: MakeRecurringBookingPaymentNotRequiredInput!\n) {\n  makeRecurringBookingPaymentNotRequired(input: $input) {\n    recurringBooking {\n      id\n      marketplaceBooking {\n        id\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e7a4027608ad2501d466be217a8f380e";

export default node;
