/**
 * @generated SignedSource<<e759b30948b1f2cfc5559442e20acc4a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type ProductOrderField = "NAME" | "%future added value";
export type ProductOrderInput = {
  direction: OrderDirection;
  field: ProductOrderField;
};
export type organizationProducts_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
  productsSortingValues?: ReadonlyArray<ProductOrderInput> | null | undefined;
};
export type organizationProducts_rootQuery$data = {
  readonly products: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly listingMetadata: {
          readonly title: string | null | undefined;
        };
        readonly organization: {
          readonly id: string;
        };
        readonly " $fragmentSpreads": FragmentRefs<"productCard_ProductDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentSpreads": FragmentRefs<"productCard_query">;
};
export type organizationProducts_rootQuery = {
  response: organizationProducts_rootQuery$data;
  variables: organizationProducts_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productsSortingValues"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "productsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Literal",
        "name": "includeInactive",
        "value": true
      },
      {
        "items": [
          {
            "kind": "Variable",
            "name": "organizationUniqueAlphanumericNames.0",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "kind": "ListValue",
        "name": "organizationUniqueAlphanumericNames"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    (v3/*: any*/)
  ],
  "storageKey": null
},
v6 = {
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
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    (v4/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subTitle",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v9 = [
  (v8/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationProducts_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ConnectionOfProductEdge",
        "kind": "LinkedField",
        "name": "products",
        "plural": false,
        "selections": [
          (v2/*: any*/),
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
                  (v3/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ListingMetadata",
                    "kind": "LinkedField",
                    "name": "listingMetadata",
                    "plural": false,
                    "selections": [
                      (v4/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v5/*: any*/),
                  {
                    "args": null,
                    "kind": "FragmentSpread",
                    "name": "productCard_ProductDetails"
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "productCard_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationProducts_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ConnectionOfProductEdge",
        "kind": "LinkedField",
        "name": "products",
        "plural": false,
        "selections": [
          (v2/*: any*/),
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
                  (v3/*: any*/),
                  (v7/*: any*/),
                  (v5/*: any*/),
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
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "featureImages",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "thumbnail",
                        "plural": false,
                        "selections": [
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
                            "name": "height",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "width",
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
                    "args": null,
                    "concreteType": "CurrencyDetails",
                    "kind": "LinkedField",
                    "name": "currency",
                    "plural": false,
                    "selections": [
                      (v8/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ProductPricing",
                    "kind": "LinkedField",
                    "name": "pricingOptions",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "index",
                        "storageKey": null
                      },
                      (v7/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "purchaseCadence",
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
                        "kind": "ScalarField",
                        "name": "isTaxInclusive",
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
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "uniqueAlphanumericName",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "canModify",
            "storageKey": null
          },
          (v3/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v9/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v9/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "573595bce2b59af971041ec7d53c5afe",
    "id": null,
    "metadata": {},
    "name": "organizationProducts_rootQuery",
    "operationKind": "query",
    "text": "query organizationProducts_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $productsSortingValues: [ProductOrderInput!]\n) {\n  products(where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: true}, orderBy: $productsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        listingMetadata {\n          title\n        }\n        organization {\n          id\n        }\n        ...productCard_ProductDetails\n      }\n    }\n  }\n  ...productCard_query\n}\n\nfragment productCard_ProductDetails on ProductDetails {\n  id\n  inactive\n  listingMetadata {\n    title\n    subTitle\n  }\n  organization {\n    id\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  currency {\n    type\n  }\n  pricingOptions {\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n  }\n}\n\nfragment productCard_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    canModify\n    id\n  }\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "8fac45d0e5fc6b51ee4b35237ef699bb";

export default node;
