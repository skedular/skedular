/**
 * @generated SignedSource<<16f638e7a51377c26baa7d35bee06143>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagOrderField = "DESCRIPTION" | "NAME" | "TYPE" | "%future added value";
export type OrganizationTagOrderInput = {
  direction: OrderDirection;
  field: OrganizationTagOrderField;
};
export type addProduct_rootQuery$variables = {
  multipleChoicesLocationTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationId: string;
};
export type addProduct_rootQuery$data = {
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesLocationTags_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoicePriceUnit_query">;
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
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "openingHoursMinutesStep",
  "storageKey": null
},
v4 = {
  "fields": [
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
},
v5 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  },
  (v4/*: any*/)
],
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "totalCount",
    "storageKey": null
  },
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v6/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "color",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "__typename",
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cursor",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
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
  {
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
  }
],
v8 = [
  "where",
  "orderBy"
],
v9 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesLocationTagsSortingValues"
  },
  (v4/*: any*/)
],
v10 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v6/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addProduct_rootQuery",
    "selections": [
      (v3/*: any*/),
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
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "addProduct_rootQuery",
    "selections": [
      (v3/*: any*/),
      {
        "alias": null,
        "args": (v5/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "productTags",
        "plural": false,
        "selections": (v7/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v5/*: any*/),
        "filters": (v8/*: any*/),
        "handle": "connection",
        "key": "multipleChoicesProductTags_productTags",
        "kind": "LinkedHandle",
        "name": "productTags"
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "ConnectionOfOrganizationTagEdge",
        "kind": "LinkedField",
        "name": "locationTags",
        "plural": false,
        "selections": (v7/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "filters": (v8/*: any*/),
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
        "selections": (v10/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v10/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "5ddad224ef72842eeb7f7ccca35e7dc7",
    "id": null,
    "metadata": {},
    "name": "addProduct_rootQuery",
    "operationKind": "query",
    "text": "query addProduct_rootQuery(\n  $organizationId: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n  $multipleChoicesLocationTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  openingHoursMinutesStep\n  ...multipleChoicesProductTags_query\n  ...multipleChoicesLocationTags_query\n  ...singleChoicePriceUnit_query\n  ...singleChoiceCurrency_query\n}\n\nfragment multipleChoicesLocationTags_query on Query {\n  locationTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesLocationTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  productTags(where: {organizationId: $organizationId}, orderBy: $multipleChoicesProductTagsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        color\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoicePriceUnit_query on Query {\n  priceUnits {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "61afce0d8a354ea1a8f734a29d05fafa";

export default node;
