/**
 * @generated SignedSource<<bca664944d7f2f05406be65fc69ab15d>>
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
  organizationCustomDomain: string;
  organizationStripeConnectAccountNameSearchText?: string | null | undefined;
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
  "name": "organizationCustomDomain"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationStripeConnectAccountNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productTagNameSearchText"
},
v4 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
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
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "companyName",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "country",
  "storageKey": null
},
v9 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v5/*: any*/)
],
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
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "productTagNameSearchText"
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
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v19 = [
  (v10/*: any*/),
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
],
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isDefault",
  "storageKey": null
},
v21 = {
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
      "name": "customDomain",
      "storageKey": null
    },
    (v6/*: any*/)
  ],
  "storageKey": null
},
v22 = [
  (v10/*: any*/),
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "organizationBankAccountNameSearchText"
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
      (v1/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/),
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
            "concreteType": "OrganizationBillingDetails",
            "kind": "LinkedField",
            "name": "billingDetails",
            "plural": false,
            "selections": [
              (v6/*: any*/),
              (v7/*: any*/),
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
                "name": "osmType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "osmId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "placeId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "longitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "latitude",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "formattedAddress",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "suburb",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "city",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "province",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zipcode",
                "storageKey": null
              },
              (v8/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "countryCode",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
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
            "selections": (v9/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "invoiceDueInDays",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationXeroConnection",
            "kind": "LinkedField",
            "name": "xeroConnection",
            "plural": false,
            "selections": [
              (v6/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "tenantId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "tenantName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "billingMode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "scopes",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "isActive",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "sendInvoicesViaXero",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "autoReconcilePayments",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultSalesAccountCode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultReceivablesAccountCode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultTrackingCategory1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultTrackingCategory2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultBrandingThemeId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "defaultReferencePrefix",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "lastSuccessfulSyncAt",
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
                "name": "hasAccessToken",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasRefreshToken",
                "storageKey": null
              }
            ],
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
            "args": (v11/*: any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": [
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
            "args": (v11/*: any*/),
            "filters": (v17/*: any*/),
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
        "selections": (v9/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationXeroBillingModeDetails",
        "kind": "LinkedField",
        "name": "organizationXeroBillingModes",
        "plural": true,
        "selections": (v9/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
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
                  (v6/*: any*/),
                  (v20/*: any*/),
                  (v5/*: any*/),
                  (v8/*: any*/),
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
                  (v7/*: any*/),
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
                  (v21/*: any*/),
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
        "args": (v19/*: any*/),
        "filters": (v17/*: any*/),
        "handle": "connection",
        "key": "organizationMarketplaceSetup_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
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
                  (v6/*: any*/),
                  (v20/*: any*/),
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
                  (v8/*: any*/),
                  (v21/*: any*/),
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
        "key": "organizationMarketplaceSetup_organizationBankAccounts",
        "kind": "LinkedHandle",
        "name": "organizationBankAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "b7bd58fce6ff5fb3da93d9de6e1600cf",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationMarketplaceSetup_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationMarketplaceSetup_rootQuery(\n  $organizationCustomDomain: String!\n  $productTagNameSearchText: String\n  $organizationStripeConnectAccountNameSearchText: String\n  $organizationBankAccountNameSearchText: String\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  ...organizationMarketplaceSetup_query\n  ...organizationMarketplaceSetup_productTags_query\n  ...organizationMarketplaceSetup_organizationStripeConnectAccounts_query\n  ...organizationMarketplaceSetup_organizationBankAccounts_query\n}\n\nfragment existingStripeConnectAccountButton_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    stripeAuthorizeExistingConnectAccountUrl\n    id\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationBankAccounts_query on Query {\n  organizationBankAccounts(where: {organizationCustomDomain: $organizationCustomDomain, nameContains: $organizationBankAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        bankName\n        accountHolderName\n        accountNumber\n        country\n        organization {\n          customDomain\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_organizationStripeConnectAccounts_query on Query {\n  organizationStripeConnectAccounts(where: {organizationCustomDomain: $organizationCustomDomain, nameContains: $organizationStripeConnectAccountNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n    totalCount\n    edges {\n      node {\n        id\n        isDefault\n        name\n        country\n        defaultCurrency\n        businessType\n        companyName\n        url\n        supportUrl\n        contactEmail\n        contactPhone\n        onboardingUrl\n        chargesEnabled\n        payoutsEnabled\n        detailsSubmitted\n        isAuthorized\n        isOnboardingCompleted\n        organization {\n          customDomain\n          id\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment organizationMarketplaceSetup_productTags_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    productTags(where: {nameContains: $productTagNameSearchText}, orderBy: [{direction: ASCENDING, field: NAME}]) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          description\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment organizationMarketplaceSetup_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n    billingDetails {\n      id\n      companyName\n      email\n      osmType\n      osmId\n      placeId\n      longitude\n      latitude\n      formattedAddress\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      countryCode\n    }\n    marketplaceListingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    billingCycle {\n      type\n      name\n    }\n    invoiceDueInDays\n    xeroConnection {\n      id\n      tenantId\n      tenantName\n      billingMode\n      scopes\n      isActive\n      sendInvoicesViaXero\n      autoReconcilePayments\n      defaultSalesAccountCode\n      defaultReceivablesAccountCode\n      defaultTrackingCategory1\n      defaultTrackingCategory2\n      defaultBrandingThemeId\n      defaultReferencePrefix\n      lastSuccessfulSyncAt\n      lastError\n      hasAccessToken\n      hasRefreshToken\n    }\n  }\n  ...existingStripeConnectAccountButton_query\n  ...singleChoiceOrganizationBillingCycle_query\n  ...singleChoiceOrganizationXeroBillingMode_query\n}\n\nfragment singleChoiceOrganizationBillingCycle_query on Query {\n  organizationBillingCycles {\n    type\n    name\n  }\n}\n\nfragment singleChoiceOrganizationXeroBillingMode_query on Query {\n  organizationXeroBillingModes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "5527ebacd603d542bf9c1ceac5ad93d4";

export default node;
