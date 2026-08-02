/**
 * @generated SignedSource<<753153cf9acde71dcaf030c091867475>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type pageOrganizationRefundDetail_rootQuery$variables = {
  refundId: string;
};
export type pageOrganizationRefundDetail_rootQuery$data = {
  readonly marketplaceRefund: {
    readonly currency: {
      readonly name: string;
      readonly type: Currency;
    } | null | undefined;
    readonly currencyToDisplay: string;
    readonly events: ReadonlyArray<{
      readonly actorName: string | null | undefined;
      readonly currencyToDisplay: string;
      readonly eventType: {
        readonly name: string;
        readonly type: MarketplaceRefundEventType;
      };
      readonly externalRefundNumber: string | null | undefined;
      readonly id: string;
      readonly lastError: string | null | undefined;
      readonly occurredAt: any;
      readonly reason: string | null | undefined;
      readonly refundAmount: any | null | undefined;
    }>;
    readonly externalRefundNumber: string | null | undefined;
    readonly id: string;
    readonly lastError: string | null | undefined;
    readonly localEntityId: string;
    readonly localEntityType: string;
    readonly reason: string | null | undefined;
    readonly refundAmount: any | null | undefined;
    readonly requestedByCustomerName: string | null | undefined;
    readonly status: {
      readonly name: string;
      readonly type: MarketplaceRefundStatus;
    };
  } | null | undefined;
};
export type pageOrganizationRefundDetail_rootQuery = {
  response: pageOrganizationRefundDetail_rootQuery$data;
  variables: pageOrganizationRefundDetail_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "refundId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v8 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "refundId"
      }
    ],
    "concreteType": "MarketplaceRefundDetails",
    "kind": "LinkedField",
    "name": "marketplaceRefund",
    "plural": false,
    "selections": [
      (v1/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "localEntityType",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "localEntityId",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currency",
        "plural": false,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceRefundStatusDetails",
        "kind": "LinkedField",
        "name": "status",
        "plural": false,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "requestedByCustomerName",
        "storageKey": null
      },
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceRefundEventDetails",
        "kind": "LinkedField",
        "name": "events",
        "plural": true,
        "selections": [
          (v1/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceRefundEventTypeDetails",
            "kind": "LinkedField",
            "name": "eventType",
            "plural": false,
            "selections": (v2/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "occurredAt",
            "storageKey": null
          },
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "actorName",
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
    "name": "pageOrganizationRefundDetail_rootQuery",
    "selections": (v8/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationRefundDetail_rootQuery",
    "selections": (v8/*:: as any*/)
  },
  "params": {
    "cacheID": "e7a95536c9a8ddd8b13a485cb74ff33d",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationRefundDetail_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationRefundDetail_rootQuery(\n  $refundId: String!\n) {\n  marketplaceRefund(id: $refundId) {\n    id\n    localEntityType\n    localEntityId\n    currency {\n      type\n      name\n    }\n    status {\n      type\n      name\n    }\n    requestedByCustomerName\n    refundAmount\n    currencyToDisplay\n    reason\n    lastError\n    externalRefundNumber\n    events {\n      id\n      eventType {\n        type\n        name\n      }\n      occurredAt\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      actorName\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7118adb1c3a5e2e53a20f31c3df657ee";

export default node;
