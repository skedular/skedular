/**
 * @generated SignedSource<<5bbee9e320fc2999bb168e7d02673abd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type FailMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason?: string | null | undefined;
};
export type marketplaceRefundAdminPanel_failMarketplaceRefundMutation$variables = {
  input: FailMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_failMarketplaceRefundMutation$data = {
  readonly failMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_failMarketplaceRefundMutation$rawResponse = {
  readonly failMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_failMarketplaceRefundMutation = {
  rawResponse: marketplaceRefundAdminPanel_failMarketplaceRefundMutation$rawResponse;
  response: marketplaceRefundAdminPanel_failMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_failMarketplaceRefundMutation$variables;
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
    "name": "failMarketplaceRefund",
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
    "name": "marketplaceRefundAdminPanel_failMarketplaceRefundMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_failMarketplaceRefundMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "8c5b554c92cf5710394f91aaefa62bb9",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_failMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_failMarketplaceRefundMutation(\n  $input: FailMarketplaceRefundInput!\n) {\n  failMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1caf5314c9cf199a96061fe5ce8fdb2f";

export default node;
