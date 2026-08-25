/**
 * @generated SignedSource<<e01d4daa82bf60de7f4aa469e8dad04e>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductSubscribe_rootQuery$variables = {
  organizationCustomDomain: string;
  productId: string;
};
export type marketplaceProductSubscribe_rootQuery$data = {
  readonly organization: {
    readonly spacesPublicBookingAvailability: {
      readonly available: boolean;
      readonly message: string;
    };
  } | null | undefined;
  readonly product: {
    readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductSubscribeHero_product">;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductSubscribeForm_query">;
};
export type marketplaceProductSubscribe_rootQuery = {
  response: marketplaceProductSubscribe_rootQuery$data;
  variables: marketplaceProductSubscribe_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productId"
},
v2 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "SpacesPublicBookingAvailabilityDetails",
  "kind": "LinkedField",
  "name": "spacesPublicBookingAvailability",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "available",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "message",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v4 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "productId"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v8/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "marketplaceProductSubscribe_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v4/*:: as any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          {
            "args": null,
            "kind": "FragmentSpread",
            "name": "marketplaceProductSubscribeHero_product"
          }
        ],
        "storageKey": null
      },
      {
        "args": [
          {
            "kind": "Variable",
            "name": "productId",
            "variableName": "productId"
          }
        ],
        "kind": "FragmentSpread",
        "name": "marketplaceProductSubscribeForm_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "marketplaceProductSubscribe_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v5/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v4/*:: as any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": [
              (v6/*:: as any*/),
              (v7/*:: as any*/),
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
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": [
              (v5/*:: as any*/),
              (v8/*:: as any*/)
            ],
            "storageKey": null
          },
          (v5/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "latestProductVersionId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v9/*:: as any*/),
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
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "customerFacingTermsAndConditionsUrl",
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
            "selections": (v9/*:: as any*/),
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
              (v5/*:: as any*/),
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
                  (v6/*:: as any*/),
                  (v7/*:: as any*/)
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
                "name": "billingMode",
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
                "name": "availableDays",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "requiredDaysPerWeek",
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
                "name": "cancellationPolicyType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "ProductPricingCancellationRefundRule",
                "kind": "LinkedField",
                "name": "cancellationRefundRules",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "minutesBefore",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "refundPercentage",
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
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v5/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "emails",
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
        "name": "currencies",
        "plural": true,
        "selections": (v9/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentMethodTypeDetails",
        "kind": "LinkedField",
        "name": "paymentMethodTypes",
        "plural": true,
        "selections": (v9/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "377c9795482108ced3ae62365cb6a840",
    "id": null,
    "metadata": {},
    "name": "marketplaceProductSubscribe_rootQuery",
    "operationKind": "query",
    "text": "query marketplaceProductSubscribe_rootQuery(\n  $productId: String!\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    spacesPublicBookingAvailability {\n      available\n      message\n    }\n    id\n  }\n  product(id: $productId) {\n    ...marketplaceProductSubscribeHero_product\n    id\n  }\n  ...marketplaceProductSubscribeForm_query_2SWcqy\n}\n\nfragment marketplaceProductSubscribeForm_query_2SWcqy on Query {\n  me {\n    id\n    emails\n  }\n  currencies {\n    type\n    name\n  }\n  paymentMethodTypes {\n    type\n    name\n  }\n  product(id: $productId) {\n    id\n    latestProductVersionId\n    type {\n      type\n      name\n    }\n    organization {\n      customerFacingTermsAndConditionsUrl\n    }\n    listingMetadata {\n      title\n    }\n    currency {\n      type\n      name\n    }\n    pricingOptions {\n      id\n      index\n      listingMetadata {\n        title\n        subTitle\n      }\n      purchaseCadence\n      price\n      isTaxInclusive\n      supportsSubscriptionAutoRenewal\n      billingMode\n      acceptedPaymentMethods\n      availableDays\n      requiredDaysPerWeek\n      numberOfResourcesToBook\n      cancellationPolicyType\n      cancellationRefundRules {\n        minutesBefore\n        refundPercentage\n      }\n    }\n  }\n}\n\nfragment marketplaceProductSubscribeHero_product on ProductDetails {\n  listingMetadata {\n    title\n    subTitle\n    about\n    includedFeatures\n  }\n  featureImages {\n    original {\n      url\n    }\n  }\n  amenities {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "9fc0301a8042b79c68b42538e458a83e";

export default node;
