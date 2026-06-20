/**
 * @generated SignedSource<<e880293787f84618b1d75e31b676aed9>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontSelectedLocationProductsQuery$variables = {
  locationId: string;
};
export type guestStoreFrontSelectedLocationProductsQuery$data = {
  readonly location: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly pricingOptions: ReadonlyArray<{
        readonly index: number;
      }>;
      readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProductCard_product">;
    }>;
  } | null | undefined;
};
export type guestStoreFrontSelectedLocationProductsQuery = {
  response: guestStoreFrontSelectedLocationProductsQuery$data;
  variables: guestStoreFrontSelectedLocationProductsQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
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
  "name": "index",
  "storageKey": null
},
v4 = {
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
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "guestStoreFrontSelectedLocationProductsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductDetails",
            "kind": "LinkedField",
            "name": "products",
            "plural": true,
            "selections": [
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductPricing",
                "kind": "LinkedField",
                "name": "pricingOptions",
                "plural": true,
                "selections": [
                  (v3/*:: as any*/)
                ],
                "storageKey": null
              },
              {
                "args": null,
                "kind": "FragmentSpread",
                "name": "guestStoreFrontProductCard_product"
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "guestStoreFrontSelectedLocationProductsQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductDetails",
            "kind": "LinkedField",
            "name": "products",
            "plural": true,
            "selections": [
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductPricing",
                "kind": "LinkedField",
                "name": "pricingOptions",
                "plural": true,
                "selections": [
                  (v3/*:: as any*/),
                  (v2/*:: as any*/),
                  (v4/*:: as any*/),
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
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "availableDays",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v4/*:: as any*/),
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
                  (v5/*:: as any*/)
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
                  (v2/*:: as any*/),
                  (v5/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v2/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "1e3fe324b699848a10df671dc340f302",
    "id": null,
    "metadata": {},
    "name": "guestStoreFrontSelectedLocationProductsQuery",
    "operationKind": "query",
    "text": "query guestStoreFrontSelectedLocationProductsQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    products {\n      id\n      pricingOptions {\n        index\n      }\n      ...guestStoreFrontProductCard_product\n    }\n    id\n  }\n}\n\nfragment guestStoreFrontProductCard_product on ProductDetails {\n  id\n  listingMetadata {\n    title\n    subTitle\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  currency {\n    type\n    name\n  }\n  amenities {\n    id\n    name\n  }\n  pricingOptions {\n    id\n    index\n    listingMetadata {\n      title\n      subTitle\n    }\n    purchaseCadence\n    price\n    isTaxInclusive\n    supportsSubscriptionAutoRenewal\n    availableDays\n  }\n}\n"
  }
};
})();

(node as any).hash = "f897809446a9be438e9d42b81d6430b2";

export default node;
