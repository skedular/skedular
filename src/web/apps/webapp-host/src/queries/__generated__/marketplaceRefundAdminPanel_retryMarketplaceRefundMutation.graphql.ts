/**
 * @generated SignedSource<<97df20cc7d3c50ca832091c82240c372>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type RetryMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type marketplaceRefundAdminPanel_retryMarketplaceRefundMutation$variables = {
  input: RetryMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_retryMarketplaceRefundMutation$data = {
  readonly retryMarketplaceRefund: {
    readonly marketplaceRefund: {
      readonly externalRefundNumber: string | null | undefined;
      readonly id: string;
      readonly lastError: string | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: MarketplaceRefundStatus;
      };
    };
  };
};
export type marketplaceRefundAdminPanel_retryMarketplaceRefundMutation = {
  response: marketplaceRefundAdminPanel_retryMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_retryMarketplaceRefundMutation$variables;
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
    "concreteType": "MarketplaceRefundPayload",
    "kind": "LinkedField",
    "name": "retryMarketplaceRefund",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceRefundDetails",
        "kind": "LinkedField",
        "name": "marketplaceRefund",
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
            "concreteType": "MarketplaceRefundStatusDetails",
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
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "lastError",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "externalRefundNumber",
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
    "name": "marketplaceRefundAdminPanel_retryMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_retryMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "acefc263c85bf9008b6c4e0d0373075a",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_retryMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_retryMarketplaceRefundMutation(\n  $input: RetryMarketplaceRefundInput!\n) {\n  retryMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2e56ff3c0ea8252919b3be0c2de45ca7";

export default node;
