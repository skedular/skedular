/**
 * @generated SignedSource<<e0ba9ead8e5195b14e281914997e7a5b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type pageOrganizationRefunds_rootQuery$variables = {
  organizationCustomDomain: string;
  statuses?: ReadonlyArray<string> | null | undefined;
};
export type pageOrganizationRefunds_rootQuery$data = {
  readonly marketplaceRefundStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: string;
  }>;
  readonly marketplaceRefunds: ReadonlyArray<{
    readonly accountingProvider: string | null | undefined;
    readonly canProcessInXero: boolean;
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
        readonly type: string;
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
    readonly lastProcessedAt: any | null | undefined;
    readonly localEntityId: string;
    readonly localEntityType: string;
    readonly reason: string | null | undefined;
    readonly refundAmount: any | null | undefined;
    readonly refundPercentage: number;
    readonly requestedAt: any;
    readonly requestedByCustomerName: string | null | undefined;
    readonly status: {
      readonly name: string;
      readonly type: string;
    };
    readonly xeroProcessingBlockedReason: string | null | undefined;
  }>;
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canModifyPaymentMethod: boolean;
  };
};
export type pageOrganizationRefunds_rootQuery = {
  response: pageOrganizationRefunds_rootQuery$data;
  variables: pageOrganizationRefunds_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "statuses"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v4 = {
  "alias": null,
  "args": [
    (v3/*:: as any*/)
  ],
  "concreteType": "OrganizationBookingPermissions",
  "kind": "LinkedField",
  "name": "organizationBookingPermissions",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canModifyPaymentMethod",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v5 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v2/*:: as any*/)
],
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "MarketplaceRefundStatusDetails",
  "kind": "LinkedField",
  "name": "marketplaceRefundStatuses",
  "plural": true,
  "selections": (v5/*:: as any*/),
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "refundAmount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "currencyToDisplay",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "reason",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "externalRefundNumber",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "lastError",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": [
    (v3/*:: as any*/),
    {
      "kind": "Variable",
      "name": "statuses",
      "variableName": "statuses"
    }
  ],
  "concreteType": "MarketplaceRefundDetails",
  "kind": "LinkedField",
  "name": "marketplaceRefunds",
  "plural": true,
  "selections": [
    (v7/*:: as any*/),
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
      "selections": (v5/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v5/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requestedAt",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requestedByCustomerName",
      "storageKey": null
    },
    (v8/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "refundPercentage",
      "storageKey": null
    },
    (v9/*:: as any*/),
    (v10/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "accountingProvider",
      "storageKey": null
    },
    (v11/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "lastProcessedAt",
      "storageKey": null
    },
    (v12/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canProcessInXero",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "xeroProcessingBlockedReason",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceRefundEventDetails",
      "kind": "LinkedField",
      "name": "events",
      "plural": true,
      "selections": [
        (v7/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundEventTypeDetails",
          "kind": "LinkedField",
          "name": "eventType",
          "plural": false,
          "selections": (v5/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "occurredAt",
          "storageKey": null
        },
        (v8/*:: as any*/),
        (v9/*:: as any*/),
        (v10/*:: as any*/),
        (v12/*:: as any*/),
        (v11/*:: as any*/),
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationRefunds_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      (v4/*:: as any*/),
      (v6/*:: as any*/),
      (v13/*:: as any*/)
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationRefunds_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
      (v4/*:: as any*/),
      (v6/*:: as any*/),
      (v13/*:: as any*/)
    ]
  },
  "params": {
    "cacheID": "b1876afe5197ecf3aabfaf563e3dfc89",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationRefunds_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationRefunds_rootQuery(\n  $organizationCustomDomain: String!\n  $statuses: [String!]\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  organizationBookingPermissions(organizationCustomDomain: $organizationCustomDomain) {\n    canModifyPaymentMethod\n  }\n  marketplaceRefundStatuses {\n    type\n    name\n  }\n  marketplaceRefunds(organizationCustomDomain: $organizationCustomDomain, statuses: $statuses) {\n    id\n    localEntityType\n    localEntityId\n    currency {\n      type\n      name\n    }\n    status {\n      type\n      name\n    }\n    requestedAt\n    requestedByCustomerName\n    refundAmount\n    refundPercentage\n    currencyToDisplay\n    reason\n    accountingProvider\n    externalRefundNumber\n    lastProcessedAt\n    lastError\n    canProcessInXero\n    xeroProcessingBlockedReason\n    events {\n      id\n      eventType {\n        type\n        name\n      }\n      occurredAt\n      refundAmount\n      currencyToDisplay\n      reason\n      lastError\n      externalRefundNumber\n      actorName\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "05f8c5e3ef1755706a590db3b97092a0";

export default node;
