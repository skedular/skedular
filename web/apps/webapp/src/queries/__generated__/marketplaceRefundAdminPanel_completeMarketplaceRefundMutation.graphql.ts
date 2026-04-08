/**
 * @generated SignedSource<<d95ec7521299232b4447eb4032c6f2bf>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type CompleteMarketplaceRefundInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason?: string | null | undefined;
};
export type marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$variables = {
  input: CompleteMarketplaceRefundInput;
};
export type marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$data = {
  readonly completeMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$rawResponse = {
  readonly completeMarketplaceRefund: {
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
export type marketplaceRefundAdminPanel_completeMarketplaceRefundMutation = {
  rawResponse: marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$rawResponse;
  response: marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$data;
  variables: marketplaceRefundAdminPanel_completeMarketplaceRefundMutation$variables;
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
    "name": "completeMarketplaceRefund",
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
    "name": "marketplaceRefundAdminPanel_completeMarketplaceRefundMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_completeMarketplaceRefundMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "00d029060db231138410d7190734814f",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_completeMarketplaceRefundMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_completeMarketplaceRefundMutation(\n  $input: CompleteMarketplaceRefundInput!\n) {\n  completeMarketplaceRefund(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f562969586568ed2587ce42e7e12dc27";

export default node;
