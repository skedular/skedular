/**
 * @generated SignedSource<<3c488022a20ca3cd9bf0c5b9fc23d6af>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type CancelMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason: string;
};
export type marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation$variables = {
  input: CancelMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation$data = {
  readonly cancelMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation = {
  response: marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation$variables;
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
    "name": "cancelMarketplaceRefund",
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
    "name": "marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "67d6611765bdbb2b012aa989e0137c3a",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_cancelMarketplaceRefundMutation(\n  $input: CancelMarketplaceRefundInput!\n) {\n  cancelMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "92b7b6e6944230514da93b3b01a69c37";

export default node;
