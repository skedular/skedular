/**
 * @generated SignedSource<<4794334d0443c865db0b3a0aae57e03c>>
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
  organizationCustomDomain: string;
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
    "name": "organizationCustomDomain"
  }
],
v1 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
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
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
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
    }
  ],
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
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
      (v3/*: any*/),
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
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "supportsSubscriptionAutoRenewal",
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  (v3/*: any*/),
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
      (v4/*: any*/)
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
      (v4/*: any*/)
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
                        "selections": (v5/*: any*/),
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
                        "name": "organizationCustomDomains.0",
                        "variableName": "organizationCustomDomain"
                      }
                    ],
                    "kind": "ListValue",
                    "name": "organizationCustomDomains"
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
                    "selections": (v5/*: any*/),
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
    "cacheID": "5a0951bf521cd39c77754470528af912",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontProductsRefetchQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontProductsRefetchQuery(\n  $locationSelected: Boolean = false\n  $organizationCustomDomain: String!\n) {\n  ...guestStoreFrontProducts_query_2IcS7w\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  listingMetadata {\n    title\n    subTitle\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n    supportsSubscriptionAutoRenewal\n  }\n}\n\nfragment guestStoreFrontProducts_query_2IcS7w on Query {\n  marketplaceLocations(where: {organizationCustomDomain: $organizationCustomDomain}) @include(if: $locationSelected) {\n    edges {\n      node {\n        id\n        products {\n          id\n          pricingOptions {\n            index\n          }\n          ...guestStoreFrontProductCard_product\n        }\n      }\n    }\n  }\n  products(where: {organizationCustomDomains: [$organizationCustomDomain], includeInactive: false}) @skip(if: $locationSelected) {\n    edges {\n      node {\n        id\n        pricingOptions {\n          index\n        }\n        ...guestStoreFrontProductCard_product\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a8764b4ddf31ff0dd2bb430ec1178bb4";

export default node;
