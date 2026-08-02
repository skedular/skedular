/**
 * @generated SignedSource<<0f11851f22585db00c1b6633f026fcf8>>
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
export type pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation$variables = {
  input: DeleteMarketplaceBookingSubscriptionInput;
};
export type pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation$data = {
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
export type pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation = {
  response: pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation$data;
  variables: pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation$variables;
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
    "name": "pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "6a808fc48a7b523af4c97afe0cc01f7f",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation(\n  $input: DeleteMarketplaceBookingSubscriptionInput!\n) {\n  deleteMarketplaceBookingSubscription(input: $input) {\n    cancellationError {\n      code\n      message\n    }\n    marketplaceBookingSubscription {\n      id\n      cancelAtPeriodEnd\n      nextRenewalAt\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "966957ab109e20d7357396cc61a0d5db";

export default node;
