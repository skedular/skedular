/**
 * @generated SignedSource<<bae358ebaf1fbb12a6b274a6d5a385e6>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductSubscribeSignIn_rootQuery$variables = {
  productId: string;
};
export type marketplaceProductSubscribeSignIn_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductSubscribeAuthGate_query">;
};
export type marketplaceProductSubscribeSignIn_rootQuery = {
  response: marketplaceProductSubscribeSignIn_rootQuery$data;
  variables: marketplaceProductSubscribeSignIn_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*:: as any*/)
],
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
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscribeSignIn_rootQuery",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "productId",
            "variableName": "productId"
          }
        ],
        "kind": "FragmentSpread",
        "name": "marketplaceProductSubscribeAuthGate_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "marketplaceProductSubscribeSignIn_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCadenceDetails",
        "kind": "LinkedField",
        "name": "productPricingCadences",
        "plural": true,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v2/*:: as any*/),
        "storageKey": null
      },
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
          (v3/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v2/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": [
              (v4/*:: as any*/),
              (v5/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "about",
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
            "selections": (v2/*:: as any*/),
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
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": [
              (v3/*:: as any*/),
              (v1/*:: as any*/)
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
              (v3/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "index",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "ListingMetadata",
                "kind": "LinkedField",
                "name": "listingMetadata",
                "plural": false,
                "selections": [
                  (v4/*:: as any*/),
                  (v5/*:: as any*/)
                ],
                "storageKey": null
              },
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
                "name": "supportsSubscriptionAutoRenewal",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "billingMode",
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
  "params": {
    "cacheID": "751c7766d210b1ba64d9c7c64ae246d7",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscribeSignIn_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductSubscribeSignIn_rootQuery(\n  $productId: String!\n) {\n  ...marketplaceProductSubscribeAuthGate_query_2SWcqy\n}\n\nfragment marketplaceProductSubscribeAuthGate_query_2SWcqy on Query {\n  productPricingCadences {\n    type\n    name\n  }\n  currencies {\n    type\n    name\n  }\n  product(id: $productId) {\n    id\n    type {\n      type\n      name\n    }\n    listingMetadata {\n      title\n      subTitle\n      about\n    }\n    currency {\n      type\n      name\n    }\n    featureImages {\n      original {\n        url\n      }\n    }\n    amenities {\n      id\n      name\n    }\n    pricingOptions {\n      id\n      index\n      listingMetadata {\n        title\n        subTitle\n      }\n      purchaseCadence\n      price\n      supportsSubscriptionAutoRenewal\n      billingMode\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e06151347e35685f117dd01695d13025";

export default node;
