/**
 * @generated SignedSource<<82f4f71bedd41e058493c33deae8e5d8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CancellationErrorCode = "INSUFFICIENT_MANAGEMENT_PERMISSION" | "INVALID_TERMINAL_STATE" | "OVERRIDE_REASON_REQUIRED" | "POLICY_RESTRICTION" | "%future added value";
export type MarketplaceBookingSubscriptionCancellationMode = "AT_PERIOD_END" | "IMMEDIATE" | "%future added value";
export type MarketplaceBookingSubscriptionStatus = "ACTIVE" | "CANCELLED" | "EXPIRED" | "PAUSED" | "RENEWAL_FAILED" | "%future added value";
export type DeleteMarketplaceBookingSubscriptionInput = {
  cancellationMode: MarketplaceBookingSubscriptionCancellationMode;
  cancellationOverrideReason?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  id: string;
};
export type editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation$variables = {
  input: DeleteMarketplaceBookingSubscriptionInput;
};
export type editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation$data = {
  readonly deleteMarketplaceBookingSubscription: {
    readonly cancellationError: {
      readonly code: CancellationErrorCode;
      readonly message: string;
    } | null | undefined;
    readonly marketplaceBookingSubscription: {
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceBookingSubscriptionStatus;
      };
    } | null | undefined;
  };
};
export type editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation = {
  response: editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation$data;
  variables: editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation$variables;
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
    "concreteType": "MarketplaceBookingSubscriptionPayload",
    "kind": "LinkedField",
    "name": "deleteMarketplaceBookingSubscription",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CancellationErrorDetails",
        "kind": "LinkedField",
        "name": "cancellationError",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "code",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "message",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceBookingSubscriptionDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingSubscription",
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
            "kind": "ScalarField",
            "name": "cancelAtPeriodEnd",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "nextRenewalAt",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceBookingSubscriptionStatusDetails",
            "kind": "LinkedField",
            "name": "status",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "d3a0b490cd4823779e95f7e871931160",
    "id": null,
    "metadata": {},
    "name": "editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation editMarketplaceBooking_deleteMarketplaceBookingSubscriptionMutation(\n  $input: DeleteMarketplaceBookingSubscriptionInput!\n) {\n  deleteMarketplaceBookingSubscription(input: $input) {\n    cancellationError {\n      code\n      message\n    }\n    marketplaceBookingSubscription {\n      id\n      cancelAtPeriodEnd\n      nextRenewalAt\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b2a5cf7db2e8b3a2ee7c1bb8c5235196";

export default node;
