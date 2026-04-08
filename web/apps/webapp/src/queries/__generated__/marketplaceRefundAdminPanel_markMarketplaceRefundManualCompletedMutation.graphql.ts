/**
 * @generated SignedSource<<6eb8d8f7574e99bd8356c6a5546dc502>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarkMarketplaceRefundManualCompletedInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason?: string | null | undefined;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$variables = {
  input: MarkMarketplaceRefundManualCompletedInput;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$data = {
  readonly markMarketplaceRefundManualCompleted: {
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
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$rawResponse = {
  readonly markMarketplaceRefundManualCompleted: {
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
export type marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation = {
  rawResponse: marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$rawResponse;
  response: marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$data;
  variables: marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation$variables;
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
    "name": "markMarketplaceRefundManualCompleted",
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
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "56690947761a410ea032e84755f31c03",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_markMarketplaceRefundManualCompletedMutation(\n  $input: MarkMarketplaceRefundManualCompletedInput!\n) {\n  markMarketplaceRefundManualCompleted(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "38854890827ab220c965dc219fac3c53";

export default node;
