/**
 * @generated SignedSource<<b198ed96ceba95db790b4c45f17bca42>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarkMarketplaceRefundManualRequiredInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason?: string | null | undefined;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$variables = {
  input: MarkMarketplaceRefundManualRequiredInput;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$data = {
  readonly markMarketplaceRefundManualRequired: {
    readonly marketplaceRefund: {
      readonly currencyToDisplay: string;
      readonly externalRefundNumber: string | null | undefined;
      readonly id: string;
      readonly lastError: string | null | undefined;
      readonly reason: string | null | undefined;
      readonly refundAmount: any | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: string;
      };
    };
  };
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$rawResponse = {
  readonly markMarketplaceRefundManualRequired: {
    readonly marketplaceRefund: {
      readonly currencyToDisplay: string;
      readonly externalRefundNumber: string | null | undefined;
      readonly id: string;
      readonly lastError: string | null | undefined;
      readonly reason: string | null | undefined;
      readonly refundAmount: any | null | undefined;
      readonly status: {
        readonly name: string;
        readonly type: string;
      };
    };
  };
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation = {
  rawResponse: marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$rawResponse;
  response: marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$data;
  variables: marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation$variables;
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
    "name": "markMarketplaceRefundManualRequired",
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
            "name": "refundAmount",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "currencyToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "reason",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f0845ef5e486890424906dd9e9233c15",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_markMarketplaceRefundManualRequiredMutation(\n  $input: MarkMarketplaceRefundManualRequiredInput!\n) {\n  markMarketplaceRefundManualRequired(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "65b8a9fc57c85be0570146480ac8312f";

export default node;
