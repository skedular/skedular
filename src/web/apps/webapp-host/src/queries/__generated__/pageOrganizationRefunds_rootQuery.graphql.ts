/**
 * @generated SignedSource<<d732dbe6af9cf467f9a773e30bb1efd5>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceRefundEventType = "ACCOUNTING_PROJECTED" | "ACCOUNTING_PROJECTION_REQUIRED" | "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "SENT_TO_XERO" | "UNDER_REVIEW" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type pageOrganizationRefunds_rootQuery$variables = {
  externalAfter?: string | null | undefined;
  externalBefore?: string | null | undefined;
  externalFirst?: number | null | undefined;
  externalLast?: number | null | undefined;
  externalProvider?: string | null | undefined;
  externalStatus?: string | null | undefined;
  organizationCustomDomain: string;
  refundAfter?: string | null | undefined;
  refundBefore?: string | null | undefined;
  refundFirst?: number | null | undefined;
  refundLast?: number | null | undefined;
  refundRequestedAtFrom?: any | null | undefined;
  refundRequestedAtTo?: any | null | undefined;
  refundStatuses?: ReadonlyArray<string> | null | undefined;
};
export type pageOrganizationRefunds_rootQuery$data = {
  readonly marketplaceExternalRefundReconciliations: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly amount: any | null | undefined;
        readonly currency: string | null | undefined;
        readonly externalRefundId: string;
        readonly lastSeenAt: any;
        readonly provider: string;
        readonly resolutionReason: string | null | undefined;
        readonly status: string;
      };
    }>;
    readonly pageInfo: {
      readonly endCursor: string | null | undefined;
      readonly hasNextPage: boolean;
      readonly hasPreviousPage: boolean;
      readonly startCursor: string | null | undefined;
    };
  };
  readonly marketplaceRefundQueue: {
    readonly edges: ReadonlyArray<{
      readonly node: {
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
        readonly paymentProvider: string | null | undefined;
        readonly reason: string | null | undefined;
        readonly refundAmount: any | null | undefined;
        readonly requestedAt: any;
        readonly requestedByCustomerName: string | null | undefined;
        readonly status: {
          readonly name: string;
          readonly type: MarketplaceRefundStatus;
        };
      };
    }>;
    readonly pageInfo: {
      readonly endCursor: string | null | undefined;
      readonly hasNextPage: boolean;
      readonly hasPreviousPage: boolean;
      readonly startCursor: string | null | undefined;
    };
  };
  readonly marketplaceRefundStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: MarketplaceRefundStatus;
  }>;
  readonly organization: {
    readonly customDomain: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type pageOrganizationRefunds_rootQuery = {
  response: pageOrganizationRefunds_rootQuery$data;
  variables: pageOrganizationRefunds_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalAfter"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalBefore"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalFirst"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalLast"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalProvider"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "externalStatus"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v7 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundAfter"
},
v8 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundBefore"
},
v9 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundFirst"
},
v10 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundLast"
},
v11 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundRequestedAtFrom"
},
v12 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundRequestedAtTo"
},
v13 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "refundStatuses"
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v16 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v15/*:: as any*/)
],
v17 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v18 = {
  "alias": null,
  "args": null,
  "concreteType": "PageInfo",
  "kind": "LinkedField",
  "name": "pageInfo",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasNextPage",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasPreviousPage",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "startCursor",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "endCursor",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v21 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v22 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v23 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v24 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "customDomain",
        "variableName": "organizationCustomDomain"
      }
    ],
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "organization",
    "plural": false,
    "selections": [
      (v14/*:: as any*/),
      (v15/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "customDomain",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "MarketplaceRefundStatusDetails",
    "kind": "LinkedField",
    "name": "marketplaceRefundStatuses",
    "plural": true,
    "selections": (v16/*:: as any*/),
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "after",
        "variableName": "refundAfter"
      },
      {
        "kind": "Variable",
        "name": "before",
        "variableName": "refundBefore"
      },
      {
        "kind": "Variable",
        "name": "first",
        "variableName": "refundFirst"
      },
      {
        "kind": "Variable",
        "name": "last",
        "variableName": "refundLast"
      },
      {
        "fields": [
          (v17/*:: as any*/),
          {
            "kind": "Variable",
            "name": "requestedAtGte",
            "variableName": "refundRequestedAtFrom"
          },
          {
            "kind": "Variable",
            "name": "requestedAtLte",
            "variableName": "refundRequestedAtTo"
          },
          {
            "kind": "Variable",
            "name": "statuses",
            "variableName": "refundStatuses"
          }
        ],
        "kind": "ObjectValue",
        "name": "where"
      }
    ],
    "concreteType": "ConnectionOfMarketplaceRefundEdge",
    "kind": "LinkedField",
    "name": "marketplaceRefundQueue",
    "plural": false,
    "selections": [
      (v18/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceRefundEdge",
        "kind": "LinkedField",
        "name": "edges",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceRefundDetails",
            "kind": "LinkedField",
            "name": "node",
            "plural": false,
            "selections": [
              (v14/*:: as any*/),
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
                "selections": (v16/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceRefundStatusDetails",
                "kind": "LinkedField",
                "name": "status",
                "plural": false,
                "selections": (v16/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "requestedAt",
                "storageKey": null
              },
              (v19/*:: as any*/),
              (v20/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "requestedByCustomerName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "paymentProvider",
                "storageKey": null
              },
              (v21/*:: as any*/),
              (v22/*:: as any*/),
              (v23/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "MarketplaceRefundEventDetails",
                "kind": "LinkedField",
                "name": "events",
                "plural": true,
                "selections": [
                  (v14/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "MarketplaceRefundEventTypeDetails",
                    "kind": "LinkedField",
                    "name": "eventType",
                    "plural": false,
                    "selections": (v16/*:: as any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "occurredAt",
                    "storageKey": null
                  },
                  (v19/*:: as any*/),
                  (v20/*:: as any*/),
                  (v21/*:: as any*/),
                  (v22/*:: as any*/),
                  (v23/*:: as any*/),
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
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "after",
        "variableName": "externalAfter"
      },
      {
        "kind": "Variable",
        "name": "before",
        "variableName": "externalBefore"
      },
      {
        "kind": "Variable",
        "name": "first",
        "variableName": "externalFirst"
      },
      {
        "kind": "Variable",
        "name": "last",
        "variableName": "externalLast"
      },
      (v17/*:: as any*/),
      {
        "kind": "Variable",
        "name": "provider",
        "variableName": "externalProvider"
      },
      {
        "kind": "Variable",
        "name": "status",
        "variableName": "externalStatus"
      }
    ],
    "concreteType": "ConnectionOfMarketplaceExternalRefundReconciliationEdge",
    "kind": "LinkedField",
    "name": "marketplaceExternalRefundReconciliations",
    "plural": false,
    "selections": [
      (v18/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "MarketplaceExternalRefundReconciliationEdge",
        "kind": "LinkedField",
        "name": "edges",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "MarketplaceExternalRefundReconciliationDetails",
            "kind": "LinkedField",
            "name": "node",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "provider",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "externalRefundId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "amount",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "currency",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "status",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "lastSeenAt",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "resolutionReason",
                "storageKey": null
              }
            ],
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
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/),
      (v7/*:: as any*/),
      (v8/*:: as any*/),
      (v9/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      (v12/*:: as any*/),
      (v13/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationRefunds_rootQuery",
    "selections": (v24/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v6/*:: as any*/),
      (v0/*:: as any*/),
      (v2/*:: as any*/),
      (v1/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v7/*:: as any*/),
      (v9/*:: as any*/),
      (v8/*:: as any*/),
      (v10/*:: as any*/),
      (v11/*:: as any*/),
      (v12/*:: as any*/),
      (v13/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationRefunds_rootQuery",
    "selections": (v24/*:: as any*/)
  },
  "params": {
    "cacheID": "7490a22e137b65f47deeb79447a9f666",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationRefunds_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationRefunds_rootQuery(\n  $organizationCustomDomain: String!\n  $externalAfter: String\n  $externalFirst: Int\n  $externalBefore: String\n  $externalLast: Int\n  $externalProvider: String\n  $externalStatus: String\n  $refundAfter: String\n  $refundFirst: Int\n  $refundBefore: String\n  $refundLast: Int\n  $refundRequestedAtFrom: DateTime\n  $refundRequestedAtTo: DateTime\n  $refundStatuses: [String!]\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    customDomain\n  }\n  marketplaceRefundStatuses {\n    type\n    name\n  }\n  marketplaceRefundQueue(after: $refundAfter, first: $refundFirst, before: $refundBefore, last: $refundLast, where: {organizationCustomDomain: $organizationCustomDomain, requestedAtGte: $refundRequestedAtFrom, requestedAtLte: $refundRequestedAtTo, statuses: $refundStatuses}) {\n    pageInfo {\n      hasNextPage\n      hasPreviousPage\n      startCursor\n      endCursor\n    }\n    edges {\n      node {\n        id\n        localEntityType\n        localEntityId\n        currency {\n          type\n          name\n        }\n        status {\n          type\n          name\n        }\n        requestedAt\n        refundAmount\n        currencyToDisplay\n        requestedByCustomerName\n        paymentProvider\n        reason\n        lastError\n        externalRefundNumber\n        events {\n          id\n          eventType {\n            type\n            name\n          }\n          occurredAt\n          refundAmount\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          actorName\n        }\n      }\n    }\n  }\n  marketplaceExternalRefundReconciliations(organizationCustomDomain: $organizationCustomDomain, after: $externalAfter, first: $externalFirst, before: $externalBefore, last: $externalLast, provider: $externalProvider, status: $externalStatus) {\n    pageInfo {\n      hasNextPage\n      hasPreviousPage\n      startCursor\n      endCursor\n    }\n    edges {\n      node {\n        provider\n        externalRefundId\n        amount\n        currency\n        status\n        lastSeenAt\n        resolutionReason\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2d5a570fd81a0709ecff82ab18f7659d";

export default node;
