/**
 * @generated SignedSource<<daa02770286afef69a2999cad5681687>>
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
export type pageOrganizationProduct_rootQuery$variables = {
  multipleChoicesLocationTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
  productId: string;
  singleChoiceOrganizationStripeConnectAccountSortingValues?: ReadonlyArray<OrganizationStripeConnectAccountOrderInput> | null | undefined;
};
export type pageOrganizationProduct_rootQuery$data = {
  readonly product: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"editProduct_query">;
};
export type pageOrganizationProduct_rootQuery = {
  response: pageOrganizationProduct_rootQuery$data;
  variables: pageOrganizationProduct_rootQuery$variables;
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
  "name": "productId"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "singleChoiceOrganizationStripeConnectAccountSortingValues"
},
v5 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "productId"
  }
],
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v6/*: any*/)
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v11 = [
  (v9/*: any*/),
  (v6/*: any*/),
  (v10/*: any*/)
],
v12 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v13 = {
  "fields": [
    (v12/*: any*/)
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v14 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  },
  (v13/*: any*/)
],
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
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
v19 = {
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
v20 = [
  (v15/*: any*/),
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
          (v7/*: any*/),
          (v6/*: any*/),
          (v10/*: any*/),
          (v16/*: any*/)
        ],
        "storageKey": null
      },
      (v17/*: any*/)
    ],
    "storageKey": null
  },
  (v18/*: any*/),
  (v19/*: any*/)
],
v21 = [
  "where",
  "orderBy"
],
v22 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesLocationTagsSortingValues"
  },
  (v13/*: any*/)
],
v23 = [
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
      (v12/*: any*/)
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
    "name": "pageOrganizationProduct_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editProduct_query"
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
      (v0/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationProduct_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v6/*: any*/),
          (v7/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "inactive",
            "storageKey": null
          },
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
            "name": "price",
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
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
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
            "concreteType": "Marketplace_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v11/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "locationTags",
            "plural": true,
            "selections": (v11/*: any*/),
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
              (v9/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Marketplace_OrganizationStripeConnectAccountDetails",
            "kind": "LinkedField",
            "name": "organizationStripeConnectAccountDetails",
            "plural": false,
            "selections": [
              (v9/*: any*/),
              (v6/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "openingHoursMinutesStep",
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v20/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v14/*: any*/),
        "filters": (v21/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesProductTags_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "concreteType": "OrganizationTagConnection",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v20/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "filters": (v21/*: any*/),
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
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v8/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v23/*: any*/),
        "concreteType": "OrganizationStripeConnectAccountConnection",
        "kind": "LinkedField",
        "name": "organizationStripeConnectAccounts",
        "plural": false,
        "selections": [
          (v15/*: any*/),
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
                  (v7/*: any*/),
                  (v6/*: any*/),
                  (v16/*: any*/)
                ],
                "storageKey": null
              },
              (v17/*: any*/)
            ],
            "storageKey": null
          },
          (v18/*: any*/),
          (v19/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v23/*: any*/),
        "filters": (v21/*: any*/),
        "handle": "connection",
        "key": "singleChoiceOrganizationStripeConnectAccount_organizationStripeConnectAccounts",
        "kind": "LinkedHandle",
        "name": "organizationStripeConnectAccounts"
      }
    ]
  },
  "params": {
    "cacheID": "056cf69ede9f87767f33df40fee99879",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationProduct_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationProduct_rootQuery(\n  $organizationId: String!\n  $productId: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]\n  $singleChoiceOrganizationStripeConnectAccountSortingValues: [OrganizationStripeConnectAccountOrderInput!]\n) {\n  product(id: $productId) {\n    name\n    id\n  }\n  ...editProduct_query\n}\n\nfragment editProduct_query on Query {\n  product(id: $productId) {\n    id\n    inactive\n    name\n    description\n    price\n    priceUnit {\n      type\n      name\n    }\n    currency {\n      type\n      name\n    }\n    numberOfResourcesToBook\n    minDurationMinutes\n    maxDurationMinutes\n    bookAllLocationResources\n    recurrenceWindowDays\n    requireConsecutiveDays\n    maxBookingSpreadDays\n    productTags {\n      uniqueId\n      name\n      color\n    }\n    locationTags {\n      uniqueId\n      name\n      color\n    }\n    organization {\n      uniqueId\n    }\n    organizationStripeConnectAccountDetails {\n      uniqueId\n      name\n    }\n  }\n  openingHoursMinutesStep\n  ...multipleChoicesProductTags_query\n  ...multipleChoicesLocationTags_query\n  ...singleChoicePriceUnit_query\n  ...singleChoiceCurrency_query\n  ...singleChoiceOrganizationStripeConnectAccount_query\n}\n\nfragment multipleChoicesLocationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesLocationTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  productTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesProductTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceOrganizationStripeConnectAccount_query on Query {\n  organizationStripeConnectAccounts(where: {organizationId: $organizationId, onboardingCompleted: true}, orderBy: $singleChoiceOrganizationStripeConnectAccountSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoicePriceUnit_query on Query {\n  priceUnits {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "1a26c1b30fac1896a66169a207b1fd8b";

export default node;
