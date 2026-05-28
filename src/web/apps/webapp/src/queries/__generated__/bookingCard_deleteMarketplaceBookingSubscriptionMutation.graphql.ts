/**
 * @generated SignedSource<<fcf24d570d37b35b237eae218868b6b9>>
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
  clientMutationId?: string | null | undefined;
  id: string;
};
export type bookingCard_deleteMarketplaceBookingSubscriptionMutation$variables = {
  input: DeleteMarketplaceBookingSubscriptionInput;
};
export type bookingCard_deleteMarketplaceBookingSubscriptionMutation$data = {
  readonly deleteMarketplaceBookingSubscription: {
    readonly marketplaceBookingSubscription: {
      readonly cancelAtPeriodEnd: boolean;
      readonly id: string;
      readonly nextRenewalAt: any | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceBookingSubscriptionStatus;
      };
    };
  };
};
export type bookingCard_deleteMarketplaceBookingSubscriptionMutation = {
  response: bookingCard_deleteMarketplaceBookingSubscriptionMutation$data;
  variables: bookingCard_deleteMarketplaceBookingSubscriptionMutation$variables;
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
    "name": "bookingCard_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "bookingCard_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "3fcb7c82246bebef806f39fc66ac453f",
    "id": null,
    "metadata": {},
    "name": "bookingCard_deleteMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_deleteMarketplaceBookingSubscriptionMutation(\n  $input: DeleteMarketplaceBookingSubscriptionInput!\n) {\n  deleteMarketplaceBookingSubscription(input: $input) {\n    marketplaceBookingSubscription {\n      id\n      cancelAtPeriodEnd\n      nextRenewalAt\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f7ed7d2fd30a8dfea4425f1bc422a8ef";

export default node;
