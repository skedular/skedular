/**
 * @generated SignedSource<<92dba0026ea531fa7e91fde55bbc636d>>
 * @lightSyntaxTransform
 * @nogrep
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
export type guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation$variables = {
  input: DeleteMarketplaceBookingSubscriptionInput;
};
export type guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation$data = {
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
export type guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation = {
  response: guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation$data;
  variables: guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation$variables;
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "a800df3f1c5fd6d1a126ed8134dfc184",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation",
    "operationKind": "mutation",
    "text": "mutation guestStoreFrontSubscriptions_deleteMarketplaceBookingSubscriptionMutation(\n  $input: DeleteMarketplaceBookingSubscriptionInput!\n) {\n  deleteMarketplaceBookingSubscription(input: $input) {\n    marketplaceBookingSubscription {\n      id\n      cancelAtPeriodEnd\n      nextRenewalAt\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ed212ae72c17893169a30abb459a61db";

export default node;
