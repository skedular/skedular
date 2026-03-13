/**
 * @generated SignedSource<<7abae75d04128305dc0f90700c7e242a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontProductsRefetchQuery$variables = {
  locationSelected?: boolean | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type guestStoreFrontProductsRefetchQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProducts_query">;
};
export type guestStoreFrontProductsRefetchQuery = {
  response: guestStoreFrontProductsRefetchQuery$data;
  variables: guestStoreFrontProductsRefetchQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": false,
    "kind": "LocalArgument",
    "name": "locationSelected"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  }
],
v1 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = [
  (v2/*: any*/),
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
      (v2/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "ListingMetadata",
        "kind": "LinkedField",
        "name": "listingMetadata",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "cadence",
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
  },
  (v5/*: any*/),
  {
    "alias": null,
    "args": null,
    "concreteType": "ListingMetadata",
    "kind": "LinkedField",
    "name": "listingMetadata",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "about",
        "storageKey": null
      },
      (v3/*: any*/),
      (v4/*: any*/),
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
        "name": "original",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "url",
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
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "type",
        "storageKey": null
      },
      (v5/*: any*/)
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationTagDetails",
    "kind": "LinkedField",
    "name": "amenities",
    "plural": true,
    "selections": [
      (v2/*: any*/),
      (v5/*: any*/)
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontProductsRefetchQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "locationSelected",
            "variableName": "locationSelected"
          },
          (v1/*: any*/)
        ],
        "kind": "FragmentSpread",
        "name": "guestStoreFrontProducts_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "guestStoreFrontProductsRefetchQuery",
    "selections": [
      {
        "condition": "locationSelected",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "fields": [
                  (v1/*: any*/)
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "ConnectionOfLocationEdge",
            "kind": "LinkedField",
            "name": "marketplaceLocations",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v2/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "ProductDetails",
                        "kind": "LinkedField",
                        "name": "products",
                        "plural": true,
                        "selections": (v6/*: any*/),
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
          }
        ]
      },
      {
        "condition": "locationSelected",
        "kind": "Condition",
        "passingValue": false,
        "selections": [
          {
            "alias": null,
            "args": [
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
            "concreteType": "ConnectionOfProductEdge",
            "kind": "LinkedField",
            "name": "products",
            "plural": false,
            "selections": [
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
                    "selections": (v6/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "01855fb5dcfccc8fe22b778dc505c34d",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontProductsRefetchQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontProductsRefetchQuery(\n  $locationSelected: Boolean = false\n  $organizationUniqueAlphanumericName: String!\n) {\n  ...guestStoreFrontProducts_query_CPnhj\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  name\n  listingMetadata {\n    about\n    title\n    subTitle\n    includedFeatures\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    cadence\n    price\n    isTaxInclusive\n  }\n}\n\nfragment guestStoreFrontProducts_query_CPnhj on Query {\n  marketplaceLocations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) @include(if: $locationSelected) {\n    edges {\n      node {\n        id\n        products {\n          id\n          pricingOptions {\n            index\n          }\n          ...guestStoreFrontProductCard_product\n        }\n      }\n    }\n  }\n  products(where: {organizationUniqueAlphanumericNames: [$organizationUniqueAlphanumericName], includeInactive: false}) @skip(if: $locationSelected) {\n    edges {\n      node {\n        id\n        pricingOptions {\n          index\n        }\n        ...guestStoreFrontProductCard_product\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8c5341534425bb0a1472dc95620a4d05";

export default node;
