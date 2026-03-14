/**
 * @generated SignedSource<<07802cc9062bb05381a641484400390b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationMarketplaceSetup_rootQuery$variables = {
  organizationBankAccountNameSearchText?: string | null | undefined;
  organizationStripeConnectAccountNameSearchText?: string | null | undefined;
  organizationUniqueAlphanumericName: string;
  productTagNameSearchText?: string | null | undefined;
};
export type pageOrganizationMarketplaceSetup_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_organizationBankAccounts_query" | "organizationMarketplaceSetup_organizationStripeConnectAccounts_query" | "organizationMarketplaceSetup_productTags_query" | "organizationMarketplaceSetup_query">;
};
export type pageOrganizationMarketplaceSetup_rootQuery = {
  response: pageOrganizationMarketplaceSetup_rootQuery$data;
  variables: pageOrganizationMarketplaceSetup_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationBankAccountNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationStripeConnectAccountNameSearchText"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productTagNameSearchText"
},
v4 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v5/*: any*/)
],
v8 = {
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "ASCENDING",
      "field": "NAME"
    }
  ]
},
v9 = [
  (v8/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productTagNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v13 = {
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
v14 = {
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
v15 = [
  "where",
  "orderBy"
],
v16 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v17 = [
  (v8/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationStripeConnectAccountNameSearchText"
      },
      (v16/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isDefault",
  "storageKey": null
},
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "uniqueAlphanumericName",
      "storageKey": null
    },
    (v6/*: any*/)
  ],
  "storageKey": null
},
v21 = [
  (v8/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationBankAccountNameSearchText"
      },
      (v16/*: any*/)
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
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMarketplaceSetup_productTags_query"
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
      (v3/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v6/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "marketplaceListingMetadata",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "about",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "subTitle",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "includedFeatures",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationBillingCycleDetails",
            "kind": "LinkedField",
            "name": "billingCycle",
            "plural": false,
            "selections": (v7/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "stripeAuthorizeExistingConnectAccountUrl",
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": [
              (v10/*: any*/),
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
                      (v6/*: any*/),
                      (v5/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "description",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "color",
                        "storageKey": null
                      },
                      (v11/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v12/*: any*/)
                ],
                "storageKey": null
              },
              (v13/*: any*/),
              (v14/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "filters": (v15/*: any*/),
            "handle": "connection",
            "key": "organizationMarketplaceSetup_productTags",
            "kind": "LinkedHandle",
            "name": "productTags"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationBillingCycleDetails",
        "kind": "LinkedField",
        "name": "organizationBillingCycles",
        "plural": true,
        "selections": (v7/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "concreteType": "ConnectionOfOrganizationStripeConnectAccountEdge",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          (v10/*: any*/),
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
                  (v6/*: any*/),
                  (v18/*: any*/),
                  (v5/*: any*/),
                  (v19/*: any*/),
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
                  (v20/*: any*/),
                  (v11/*: any*/)
                ],
                "storageKey": null
              },
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          (v13/*: any*/),
          (v14/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "filters": (v15/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "concreteType": "ConnectionOfOrganizationBankAccountEdge",
        "kind": "LinkedField",
        "name": "organizationBankAccounts",
        "plural": false,
        "selections": [
          (v10/*: any*/),
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
                  (v6/*: any*/),
                  (v18/*: any*/),
                  (v5/*: any*/),
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
                  (v19/*: any*/),
                  (v20/*: any*/),
                  (v11/*: any*/)
                ],
                "storageKey": null
              },
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          (v13/*: any*/),
          (v14/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v21/*: any*/),
        "filters": (v15/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationBankAccounts",
        "kind": "LinkedHandle",
        "name": "organizationBankAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "b4bb93a7c1266ac30d9eadc856222965",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationMarketplaceSetup_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $productTagNameSearchText: String\n  $organizationStripeConnectAccountNameSearchText: String\n  $organizationBankAccountNameSearchText: String\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n    id\n  }\n  ...organizationMarketplaceSetup_query\n  ...organizationMarketplaceSetup_productTags_query\n  ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query\n  ...organizationMarketplaceSetup_organizationBankAccounts_query\n}\n\nfragment existingStripeConnectAccountButton_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    stripeAuthorizeExistingConnectAccountUrl\n    id\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationBankAccounts_query on Query {\n  organizationBankAccounts(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $organizationBankAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        bankName\n        accountHolderName\n        accountNumber\n        country\n        organization {\n          uniqueAlphanumericName\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationStripeConnectAccounts_query on Query {\n  organizationStripeConnectAccounts(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $organizationStripeConnectAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        country\n        defaultCurrency\n        businessType\n        companyName\n        url\n        supportUrl\n        contactEmail\n        contactPhone\n        onboardingUrl\n        chargesEnabled\n        payoutsEnabled\n        detailsSubmitted\n        isAuthorized\n        isOnboardingCompleted\n        organization {\n          uniqueAlphanumericName\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_productTags_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    productTags(where: {nameContains: $productTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          description\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment organizationMarketplaceSetup_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    id\n    name\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    billingCycle {\n      type\n      name\n    }\n  }\n  ...existingStripeConnectAccountButton_query\n  ...singleChoiceOrganizationBillingCycle_query\n}\n\nfragment singleChoiceOrganizationBillingCycle_query on Query {\n  organizationBillingCycles {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "662208527cd44515f135d106ccf706d1";

export default node;
