/**
 * @generated SignedSource<<727f60dc6060de827ea0fe671e4b7289>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationMarketplaceSetup_rootQuery$variables = {
  locationTagNameSearchText?: string | null | undefined;
  organizationBankAccountNameSearchText?: string | null | undefined;
  organizationId: string;
  organizationStripeConnectAccountNameSearchText?: string | null | undefined;
  productNameSearchText?: string | null | undefined;
  productTagNameSearchText?: string | null | undefined;
};
export type pageOrganizationMarketplaceSetup_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_locationTags_query" | "organizationMarketplaceSetup_organizationBankAccounts_query" | "organizationMarketplaceSetup_organizationStripeConnectAccounts_query" | "organizationMarketplaceSetup_productTags_query" | "organizationMarketplaceSetup_products_query">;
};
export type pageOrganizationMarketplaceSetup_rootQuery = {
  response: pageOrganizationMarketplaceSetup_rootQuery$data;
  variables: pageOrganizationMarketplaceSetup_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationTagNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationBankAccountNameSearchText"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationStripeConnectAccountNameSearchText"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productNameSearchText"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productTagNameSearchText"
},
v6 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = [
  (v7/*: any*/)
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "ASCENDING",
      "field": "NAME"
    }
  ]
},
v11 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Literal",
        "name": "includeInactive",
        "value": true
      },
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productNameSearchText"
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "organizationIds.0",
            "variableName": "organizationId"
          }
        ],
        "kind": "ListValue",
        "name": "organizationIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "description",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v16 = {
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
      "name": "endCursor",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasNextPage",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v17 = {
  "kind": "ClientExtension",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "__id",
      "storageKey": null
    }
  ]
},
v18 = [
  "where",
  "orderBy"
],
v19 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v20 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productTagNameSearchText"
      },
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v21 = [
  (v12/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagEdge",
    "kind": "LinkedField",
    "name": "edges",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTagDetails",
        "kind": "LinkedField",
        "name": "node",
        "plural": false,
        "selections": [
          (v9/*: any*/),
          (v7/*: any*/),
          (v13/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
            "storageKey": null
          },
          (v14/*: any*/)
        ],
        "storageKey": null
      },
      (v15/*: any*/)
    ],
    "storageKey": null
  },
  (v16/*: any*/),
  (v17/*: any*/)
],
v22 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationTagNameSearchText"
      },
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v23 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationStripeConnectAccountNameSearchText"
      },
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v24 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isDefault",
  "storageKey": null
},
v25 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v26 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    (v9/*: any*/)
  ],
  "storageKey": null
},
v27 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationBankAccountNameSearchText"
      },
      (v19/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_products_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_productTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_locationTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_organizationStripeConnectAccounts_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_organizationBankAccounts_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v0/*: any*/),
      (v3/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v9/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "ConnectionOfProductEdge",
        "kind": "LinkedField",
        "name": "products",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v9/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "inactive",
                    "storageKey": null
                  },
                  (v7/*: any*/),
                  (v13/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "priceToDisplay",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "PriceUnitDetails",
                    "kind": "LinkedField",
                    "name": "priceUnit",
                    "plural": false,
                    "selections": (v8/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "numberOfResourcesToBook",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "minDurationMinutes",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "maxDurationMinutes",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "bookAllLocationResources",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "recurrenceWindowDays",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "requireConsecutiveDays",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "maxBookingSpreadDays",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Marketplace_OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "uniqueId",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  (v14/*: any*/)
                ],
                "storageKey": null
              },
              (v15/*: any*/)
            ],
            "storageKey": null
          },
          (v16/*: any*/),
          (v17/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "filters": (v18/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_products",
        "kind": "LinkedHandle",
        "name": "products"
      },
      {
        "alias": null,
        "args": (v20/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v21/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v20/*: any*/),
        "filters": (v18/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v21/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "filters": (v18/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_locationTags",
        "kind": "LinkedHandle",
        "name": "locationTags"
      },
      {
        "alias": null,
        "args": (v23/*: any*/),
        "concreteType": "ConnectionOfOrganizationStripeConnectAccountEdge",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationStripeConnectAccountEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationStripeConnectAccountDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v9/*: any*/),
                  (v24/*: any*/),
                  (v7/*: any*/),
                  (v25/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "defaultCurrency",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "businessType",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "companyName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "url",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "supportUrl",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "contactEmail",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "contactPhone",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "onboardingUrl",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "chargesEnabled",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "payoutsEnabled",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "detailsSubmitted",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "isAuthorized",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "isOnboardingCompleted",
                    "storageKey": null
                  },
                  (v26/*: any*/),
                  (v14/*: any*/)
                ],
                "storageKey": null
              },
              (v15/*: any*/)
            ],
            "storageKey": null
          },
          (v16/*: any*/),
          (v17/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v23/*: any*/),
        "filters": (v18/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "concreteType": "ConnectionOfOrganizationBankAccountEdge",
        "kind": "LinkedField",
        "name": "organizationBankAccounts",
        "plural": false,
        "selections": [
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBankAccountEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationBankAccountDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v9/*: any*/),
                  (v24/*: any*/),
                  (v7/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "bankName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "accountHolderName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "accountNumber",
                    "storageKey": null
                  },
                  (v25/*: any*/),
                  (v26/*: any*/),
                  (v14/*: any*/)
                ],
                "storageKey": null
              },
              (v15/*: any*/)
            ],
            "storageKey": null
          },
          (v16/*: any*/),
          (v17/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v27/*: any*/),
        "filters": (v18/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationBankAccounts",
        "kind": "LinkedHandle",
        "name": "organizationBankAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "642706c4de3b6e4641c4a007ec129c93",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationMarketplaceSetup_rootQuery(\n  $organizationId: String!\n  $productNameSearchText: String\n  $productTagNameSearchText: String\n  $locationTagNameSearchText: String\n  $organizationStripeConnectAccountNameSearchText: String\n  $organizationBankAccountNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationMarketplaceSetup_products_query\n  ...organizationMarketplaceSetup_productTags_query\n  ...organizationMarketplaceSetup_locationTags_query\n  ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query\n  ...organizationMarketplaceSetup_organizationBankAccounts_query\n}\n\nfragment organizationMarketplaceSetup_locationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId, nameContains: $locationTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationBankAccounts_query on Query {\n  organizationBankAccounts(where: {organizationId: $organizationId, nameContains: $organizationBankAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        bankName\n        accountHolderName\n        accountNumber\n        country\n        organization {\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationStripeConnectAccounts_query on Query {\n  organizationStripeConnectAccounts(where: {organizationId: $organizationId, nameContains: $organizationStripeConnectAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        country\n        defaultCurrency\n        businessType\n        companyName\n        url\n        supportUrl\n        contactEmail\n        contactPhone\n        onboardingUrl\n        chargesEnabled\n        payoutsEnabled\n        detailsSubmitted\n        isAuthorized\n        isOnboardingCompleted\n        organization {\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_productTags_query on Query {\n  productTags(where: {organizationId: $organizationId, nameContains: $productTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_products_query on Query {\n  products(where: {organizationIds: [$organizationId], nameContains: $productNameSearchText, includeInactive: true}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        inactive\n        name\n        description\n        priceToDisplay\n        priceUnit {\n          name\n        }\n        numberOfResourcesToBook\n        minDurationMinutes\n        maxDurationMinutes\n        bookAllLocationResources\n        recurrenceWindowDays\n        requireConsecutiveDays\n        maxBookingSpreadDays\n        organization {\n          uniqueId\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a1259549582d24dd3f4522711acf09a1";

export default node;
