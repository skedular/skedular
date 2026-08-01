/**
 * @generated SignedSource<<e09697cce5fb70d55e716a19fdb96f16>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type RejectMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason: string;
};
export type marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation$variables = {
  input: RejectMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation$data = {
  readonly rejectMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation = {
  response: marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation$variables;
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
    "name": "rejectMarketplaceRefund",
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
    "name": "marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "3dc1e2a65f47373f00ad903b50f9c239",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_rejectMarketplaceRefundMutation(\n  $input: RejectMarketplaceRefundInput!\n) {\n  rejectMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8f6c0e646658157df354e3ae19c18811";

export default node;
