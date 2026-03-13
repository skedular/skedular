/**
 * @generated SignedSource<<f49005c8bdddbd1bd75cb59a3cb130aa>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductDetail_rootQuery$variables = {
  productId: string;
};
export type marketplaceProductDetail_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailBookingCard_query" | "marketplaceProductDetailOverview_query">;
};
export type marketplaceProductDetail_rootQuery = {
  response: marketplaceProductDetail_rootQuery$data;
  variables: marketplaceProductDetail_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "productId",
    "variableName": "productId"
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
  "name": "name",
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
v5 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductDetail_rootQuery",
    "selections": [
      {
        "args": (v1/*: any*/),
        "kind": "FragmentSpread",
        "name": "marketplaceProductDetailOverview_query"
      },
      {
        "args": (v1/*: any*/),
        "kind": "FragmentSpread",
        "name": "marketplaceProductDetailBookingCard_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "marketplaceProductDetail_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "productId"
          }
        ],
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
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
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "color",
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
            "selections": (v5/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "index",
                "storageKey": null
              },
              (v3/*: any*/),
              (v4/*: any*/),
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
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "acceptedPaymentMethods",
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
                "name": "numberOfResourcesToBook",
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
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v5/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v5/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "33b7eea79bd915c467bbd6e581f6cc27",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductDetail_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductDetail_rootQuery(\n  $productId: String!\n) {\n  ...marketplaceProductDetailOverview_query_2SWcqy\n  ...marketplaceProductDetailBookingCard_query_2SWcqy\n}\n\nfragment marketplaceProductDetailBookingCard_product on ProductDetails {\n  id\n  name\n  listingMetadata {\n    about\n    title\n    subTitle\n    includedFeatures\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  amenities {\n    id\n    name\n    color\n  }\n  currency {\n    type\n    name\n  }\n  pricingOptions {\n    id\n    index\n    name\n    listingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    cadence\n    price\n    isTaxInclusive\n    acceptedPaymentMethods\n    minDurationMinutes\n    maxDurationMinutes\n    numberOfResourcesToBook\n  }\n}\n\nfragment marketplaceProductDetailBookingCard_query_2SWcqy on Query {\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n  product(id: $productId) {\n    ...marketplaceProductDetailBookingCard_product\n    id\n  }\n}\n\nfragment marketplaceProductDetailOverview_product on ProductDetails {\n  id\n  name\n  listingMetadata {\n    about\n    title\n    subTitle\n    includedFeatures\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  amenities {\n    id\n    name\n    color\n  }\n  currency {\n    type\n    name\n  }\n  pricingOptions {\n    id\n    index\n    name\n    listingMetadata {\n      about\n      title\n      subTitle\n      includedFeatures\n    }\n    cadence\n    price\n    isTaxInclusive\n    acceptedPaymentMethods\n    minDurationMinutes\n    maxDurationMinutes\n    numberOfResourcesToBook\n  }\n}\n\nfragment marketplaceProductDetailOverview_query_2SWcqy on Query {\n  product(id: $productId) {\n    ...marketplaceProductDetailOverview_product\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a9e9b09da7b93dff6f4b801f2ca8f631";

export default node;
