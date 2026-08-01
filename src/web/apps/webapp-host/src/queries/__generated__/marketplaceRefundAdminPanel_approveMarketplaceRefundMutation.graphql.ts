/**
 * @generated SignedSource<<d0c386d5eaf37f477e32cd06bf4180cb>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type ApproveMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type marketplaceRefundAdminPanel_approveMarketplaceRefundMutation$variables = {
  input: ApproveMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_approveMarketplaceRefundMutation$data = {
  readonly approveMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_approveMarketplaceRefundMutation = {
  response: marketplaceRefundAdminPanel_approveMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_approveMarketplaceRefundMutation$variables;
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
    "name": "approveMarketplaceRefund",
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
    "name": "marketplaceRefundAdminPanel_approveMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_approveMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "e2a451f1f1d819524485cc42bbfb8908",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_approveMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_approveMarketplaceRefundMutation(\n  $input: ApproveMarketplaceRefundInput!\n) {\n  approveMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "13328ca3027324aae8fc810cdf85652c";

export default node;
