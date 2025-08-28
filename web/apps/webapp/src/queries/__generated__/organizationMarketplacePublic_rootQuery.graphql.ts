/**
 * @generated SignedSource<<1e395deda32e7bbda52ab36e696bd5b8>>
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
export type organizationMarketplacePublic_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
  productsSortingValues?: ReadonlyArray<ProductOrderInput> | null | undefined;
};
export type organizationMarketplacePublic_rootQuery$data = {
  readonly products: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
        readonly organization: {
          readonly uniqueId: string;
        };
        readonly " $fragmentSpreads": FragmentRefs<"productCard_ProductDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
};
export type organizationMarketplacePublic_rootQuery = {
  response: organizationMarketplacePublic_rootQuery$data;
  variables: organizationMarketplacePublic_rootQuery$variables;
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
        "value": false
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
  "name": "name",
  "storageKey": null
},
v5 = {
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMarketplacePublic_rootQuery",
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
                  (v4/*: any*/),
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
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMarketplacePublic_rootQuery",
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
                  (v4/*: any*/),
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
                    "selections": [
                      (v4/*: any*/)
                    ],
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
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "primaryFeatureImage",
                    "plural": false,
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
                    "kind": "ScalarField",
                    "name": "isPriceTaxInclusive",
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
      }
    ]
  },
  "params": {
    "cacheID": "9a39df15aa8bf006e05b3378b4750647",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplacePublic_rootQuery",
    "operationKind": "query",
    "text": "query organizationMarketplacePublic_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $productsSortingValues: [ProductOrderInput!]\n) {\n  products(where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: false}, orderBy: $productsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueId\n        }\n        ...productCard_ProductDetails\n      }\n    }\n  }\n}\n\nfragment productCard_ProductDetails on ProductDetails {\n  id\n  name\n  description\n  priceToDisplay\n  priceUnit {\n    name\n  }\n  numberOfResourcesToBook\n  minDurationMinutes\n  maxDurationMinutes\n  requireConsecutiveDays\n  maxBookingSpreadDays\n  organization {\n    uniqueId\n  }\n  primaryFeatureImage {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  isPriceTaxInclusive\n}\n"
  }
};
})();

(node as any).hash = "11004a45c1620390c09f0544fcb39f62";

export default node;
