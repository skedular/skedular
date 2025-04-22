/**
 * @generated SignedSource<<1ae1fb8ebcc27b5ff4d47c0ebe06c5b1>>
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
  organizationId: string;
  organizationStripeConnectAccountNameSearchText?: string | null | undefined;
  productNameSearchText?: string | null | undefined;
  productTagNameSearchText?: string | null | undefined;
};
export type pageOrganizationMarketplaceSetup_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_locationTags_query" | "organizationMarketplaceSetup_organizationStripeConnectAccounts_query" | "organizationMarketplaceSetup_productTags_query" | "organizationMarketplaceSetup_products_query">;
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
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationStripeConnectAccountNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productNameSearchText"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productTagNameSearchText"
},
v5 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = [
  (v6/*: any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v9 = {
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "Ascending",
      "field": "Name"
    }
  ]
},
v10 = [
  (v9/*: any*/),
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
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "description",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v15 = {
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
v16 = {
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
v17 = [
  "where",
  "orderBy"
],
v18 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v19 = [
  (v9/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productTagNameSearchText"
      },
      (v18/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v20 = [
  (v11/*: any*/),
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
          (v8/*: any*/),
          (v6/*: any*/),
          (v12/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
            "storageKey": null
          },
          (v13/*: any*/)
        ],
        "storageKey": null
      },
      (v14/*: any*/)
    ],
    "storageKey": null
  },
  (v15/*: any*/),
  (v16/*: any*/)
],
v21 = [
  (v9/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationTagNameSearchText"
      },
      (v18/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v22 = [
  (v9/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationStripeConnectAccountNameSearchText"
      },
      (v18/*: any*/)
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
      (v4/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v7/*: any*/),
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
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v6/*: any*/),
          (v8/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v10/*: any*/),
        "concreteType": "ProductConnection",
        "kind": "LinkedField",
        "name": "products",
        "plural": false,
        "selections": [
          (v11/*: any*/),
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
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "inactive",
                    "storageKey": null
                  },
                  (v6/*: any*/),
                  (v12/*: any*/),
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
                    "selections": (v7/*: any*/),
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
                  (v13/*: any*/)
                ],
                "storageKey": null
              },
              (v14/*: any*/)
            ],
            "storageKey": null
          },
          (v15/*: any*/),
          (v16/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v10/*: any*/),
        "filters": (v17/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_products",
        "kind": "LinkedHandle",
        "name": "products"
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v20/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "filters": (v17/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v20/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "filters": (v17/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_locationTags",
        "kind": "LinkedHandle",
        "name": "locationTags"
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountConnection",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          (v11/*: any*/),
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
                  (v8/*: any*/),
                  (v6/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "country",
                    "storageKey": null
                  },
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
                    "name": "email",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "phone",
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
                    "name": "onboardingCompletedAt",
                    "storageKey": null
                  },
                  (v13/*: any*/)
                ],
                "storageKey": null
              },
              (v14/*: any*/)
            ],
            "storageKey": null
          },
          (v15/*: any*/),
          (v16/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "filters": (v17/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "8853117976a40d2f20f0d0e6a06ff67c",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationMarketplaceSetup_rootQuery(\n  $organizationId: String!\n  $productNameSearchText: String\n  $productTagNameSearchText: String\n  $locationTagNameSearchText: String\n  $organizationStripeConnectAccountNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    name\n    id\n  }\n  ...organizationMarketplaceSetup_products_query\n  ...organizationMarketplaceSetup_productTags_query\n  ...organizationMarketplaceSetup_locationTags_query\n  ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query\n}\n\nfragment organizationMarketplaceSetup_locationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId, nameContains: $locationTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationStripeConnectAccounts_query on Query {\n  organizationStripeConnectAccounts(where: {organizationId: $organizationId, nameContains: $organizationStripeConnectAccountNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        country\n        defaultCurrency\n        businessType\n        companyName\n        email\n        phone\n        onboardingUrl\n        onboardingCompletedAt\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_productTags_query on Query {\n  productTags(where: {organizationId: $organizationId, nameContains: $productTagNameSearchText}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        description\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_products_query on Query {\n  products(where: {organizationIds: [$organizationId], nameContains: $productNameSearchText, includeInactive: true}, orderBy: [{direction: Ascending, field: Name}]) {\n    totalCount\n    edges {\n      node {\n        id\n        inactive\n        name\n        description\n        priceToDisplay\n        priceUnit {\n          name\n        }\n        numberOfResourcesToBook\n        minDurationMinutes\n        maxDurationMinutes\n        bookAllLocationResources\n        recurrenceWindowDays\n        requireConsecutiveDays\n        maxBookingSpreadDays\n        organization {\n          uniqueId\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9c64b9a7ba629572d02c518fdea50558";

export default node;
