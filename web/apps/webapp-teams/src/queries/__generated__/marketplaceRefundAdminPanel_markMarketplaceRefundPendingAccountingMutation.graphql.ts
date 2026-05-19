/**
 * @generated SignedSource<<f75ec318e891a1a88239aaa09cf75c99>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type MarkMarketplaceRefundPendingAccountingInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  reason?: string | null | undefined;
  refundAmount?: any | null | undefined;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$variables = {
  input: MarkMarketplaceRefundPendingAccountingInput;
};
export type marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$data = {
  readonly markMarketplaceRefundPendingAccounting: {
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
export type marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$rawResponse = {
  readonly markMarketplaceRefundPendingAccounting: {
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
export type marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation = {
  rawResponse: marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$rawResponse;
  response: marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$data;
  variables: marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation$variables;
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
    "name": "markMarketplaceRefundPendingAccounting",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "b1c7b9d0b3698aaab965c788634559fc",
    "id": null,
    "metadata": {},
    "name": "marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation",
    "operationKind": "mutation",
    "text": "mutation marketplaceRefundAdminPanel_markMarketplaceRefundPendingAccountingMutation(\n  $input: MarkMarketplaceRefundPendingAccountingInput!\n) {\n  markMarketplaceRefundPendingAccounting(input: $input) {\n    marketplaceRefund {\n      id\n      status {\n        type\n        name\n      }\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "65db25e01bca7612c3694037ab372d72";

export default node;
