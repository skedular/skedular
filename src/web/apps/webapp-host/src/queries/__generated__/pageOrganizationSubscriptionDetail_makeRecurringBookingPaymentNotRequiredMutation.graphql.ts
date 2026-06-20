/**
 * @generated SignedSource<<17adc7741be96c54f820b49a7c8bbc1f>>
 * @lightSyntaxTransform
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
export type pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$variables = {
  input: MakeRecurringBookingPaymentNotRequiredInput;
};
export type pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$data = {
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
    } | null | undefined;
  };
};
export type pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$rawResponse = {
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
    } | null | undefined;
  };
};
export type pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation = {
  rawResponse: pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$rawResponse;
  response: pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$data;
  variables: pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation$variables;
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
    "name": "pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation",
    "selections": (v2/*:: as any*/)
  },
  "params": {
    "cacheID": "e0fd52ef7be962d17ce79bbc26d81e5e",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptionDetail_makeRecurringBookingPaymentNotRequiredMutation(\n  $input: MakeRecurringBookingPaymentNotRequiredInput!\n) {\n  makeRecurringBookingPaymentNotRequired(input: $input) {\n    recurringBooking {\n      id\n      marketplaceBooking {\n        id\n        paymentStatus {\n          type\n          name\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e19b3918533a2b606bb32b6b3a2d87c4";

export default node;
