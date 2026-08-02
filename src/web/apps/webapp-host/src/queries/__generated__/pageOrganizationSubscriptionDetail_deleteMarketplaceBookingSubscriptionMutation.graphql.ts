/**
 * @generated SignedSource<<3d1eb08eaa23b1e60797a2d724e14e5b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
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
    "cacheID": "8161738e559f2dd94053e1f6320a163d",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation pageOrganizationSubscriptionDetail_deleteMarketplaceBookingSubscriptionMutation(\n  $input: DeleteMarketplaceBookingSubscriptionInput!\n) {\n  deleteMarketplaceBookingSubscription(input: $input) {\n    marketplaceBookingSubscription {\n      id\n      cancelAtPeriodEnd\n      nextRenewalAt\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ca8b4079c8dde5f889b7c630160c1811";

export default node;
