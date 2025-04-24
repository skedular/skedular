/**
 * @generated SignedSource<<4d8a3a940211de201011b6dca482fb25>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationStripeConnectAccountOrderField = "Name" | "%future added value";
export type OrganizationTagOrderField = "Description" | "Name" | "Type" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type OrganizationStripeConnectAccountOrderInput = {
  direction: OrderDirection;
  field: OrganizationStripeConnectAccountOrderField;
};
export type addProduct_rootQuery$variables = {
  multipleChoicesLocationTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
  singleChoiceOrganizationStripeConnectAccountSortingValues?: ReadonlyArray<OrganizationStripeConnectAccountOrderInput> | null | undefined;
};
export type addProduct_rootQuery$data = {
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesLocationTags_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoiceOrganizationStripeConnectAccount_query" | "singleChoicePriceUnit_query">;
};
export type addProduct_rootQuery = {
  response: addProduct_rootQuery$data;
  variables: addProduct_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesLocationTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "multipleChoicesProductTagsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "singleChoiceOrganizationStripeConnectAccountSortingValues"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "openingHoursMinutesStep",
  "storageKey": null
},
v5 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v6 = {
  "fields": [
    (v5/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v7 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  },
  (v6/*: any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
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
  (v8/*: any*/),
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
          (v10/*: any*/),
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
v16 = [
  "where",
  "orderBy"
],
v17 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesLocationTagsSortingValues"
  },
  (v6/*: any*/)
],
v18 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v10/*: any*/)
],
v19 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "singleChoiceOrganizationStripeConnectAccountSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Literal",
        "name": "onboardingCompleted",
        "value": true
      },
      (v5/*: any*/)
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
    "name": "addProduct_rootQuery",
    "selections": [
      (v4/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesProductTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "multipleChoicesLocationTags_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoicePriceUnit_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceCurrency_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceOrganizationStripeConnectAccount_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "addProduct_rootQuery",
    "selections": [
      (v4/*: any*/),
      {
        "alias": null,
        "args": (v7/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v7/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesProductTags_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v15/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v17/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesLocationTags_locationTags",
        "kind": "LinkedHandle",
        "name": "locationTags"
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PriceUnitDetails",
        "kind": "LinkedField",
        "name": "priceUnits",
        "plural": true,
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v18/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v19/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountConnection",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          (v8/*: any*/),
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
                  (v10/*: any*/),
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
        "args": (v19/*: any*/),
        "filters": (v16/*: any*/),
        "handle": "connection",
        "key": "singleChoiceOrganizationStripeConnectAccount_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "b11e3e7e914d5e10889513bc0f0d6db3",
    "id": null,
    "metadata": {},
    "name": "addProduct_rootQuery",
    "operationKind": "query",
    "text": "query addProduct_rootQuery(\n  $organizationId: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]\n  $singleChoiceOrganizationStripeConnectAccountSortingValues: [OrganizationStripeConnectAccountOrderInput!]\n) {\n  openingHoursMinutesStep\n  ...multipleChoicesProductTags_query\n  ...multipleChoicesLocationTags_query\n  ...singleChoicePriceUnit_query\n  ...singleChoiceCurrency_query\n  ...singleChoiceOrganizationStripeConnectAccount_query\n}\n\nfragment multipleChoicesLocationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesLocationTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  productTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesProductTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceOrganizationStripeConnectAccount_query on Query {\n  organizationStripeConnectAccounts(where: {organizationId: $organizationId, onboardingCompleted: true}, orderBy: $singleChoiceOrganizationStripeConnectAccountSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoicePriceUnit_query on Query {\n  priceUnits {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "f5f51f7f9b3003e98a30b27dc1795021";

export default node;
