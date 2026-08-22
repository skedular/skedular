/**
 * @generated SignedSource<<3420e5709ee822e45dd104efa72e64e3>>
 * @lightSyntaxTransform
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
export type pageOrganizationProduct_rootQuery$variables = {
  multipleChoicesProductTagsSortingValues?: ReadonlyArray<OrganizationTagOrderInput> | null | undefined;
  organizationCustomDomain: string;
  productId: string;
};
export type pageOrganizationProduct_rootQuery$data = {
  readonly product: {
    readonly listingMetadata: {
      readonly title: string | null | undefined;
    };
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
  "name": "multipleChoicesProductTagsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "productId"
},
v3 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "productId"
  }
],
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
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
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
  (v7/*:: as any*/)
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v10 = [
  (v6/*:: as any*/),
  (v7/*:: as any*/),
  (v9/*:: as any*/)
],
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": (v10/*:: as any*/),
  "storageKey": null
},
v12 = [
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
v13 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "multipleChoicesProductTagsSortingValues"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationProduct_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*:: as any*/),
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
              (v4/*:: as any*/)
            ],
            "storageKey": null
          }
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
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v0/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationProduct_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v3/*:: as any*/),
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
              (v4/*:: as any*/),
              (v5/*:: as any*/),
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
          (v6/*:: as any*/),
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
            "concreteType": "ProductTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v8/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v8/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v10/*:: as any*/),
            "storageKey": null
          },
          (v11/*:: as any*/),
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
                "selections": (v12/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v12/*:: as any*/),
                "storageKey": null
              }
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
              (v6/*:: as any*/),
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
                "name": "supportsSubscriptionAutoRenewal",
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
                "name": "bookingCadence",
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
                "name": "minDurationMinutes",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "minDurationDisplayUnit",
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
                "name": "maxDurationDisplayUnit",
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
                    "name": "displayUnit",
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
                "name": "maxAllowedResourcesLockTimePaidViaCard",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maxAllowedResourcesLockTimePaidViaCardDisplayUnit",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maxAllowedResourcesLockTimePaidViaBankTransfer",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit",
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
                "name": "fulfillmentType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "entitlementCreditQuantity",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "entitlementValidityDays",
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
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingBillingModeDetails",
        "kind": "LinkedField",
        "name": "productPricingBillingModes",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "CurrencyDetails",
        "kind": "LinkedField",
        "name": "currencies",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "bookingSlotSizeInMinutes",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "defaultMaxAllowedResourcesLockTimePaidViaCard",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "defaultMaxAllowedResourcesLockTimePaidViaBankTransfer",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "DurationDisplayUnitDetails",
        "kind": "LinkedField",
        "name": "durationDisplayUnits",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": (v13/*:: as any*/),
            "concreteType": "ConnectionOfOrganizationTagEdge",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": false,
            "selections": [
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
                      (v6/*:: as any*/),
                      (v7/*:: as any*/),
                      (v9/*:: as any*/),
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
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v13/*:: as any*/),
            "filters": [
              "orderBy"
            ],
            "handle": "connection",
            "key": "multipleChoicesProductTags_productTags",
            "kind": "LinkedHandle",
            "name": "productTags"
          },
          (v6/*:: as any*/),
          (v11/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "PaymentMethodTypeDetails",
        "kind": "LinkedField",
        "name": "paymentMethodTypes",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductPricingCancellationTypeDetails",
        "kind": "LinkedField",
        "name": "productPricingCancellationTypes",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductTypeDetails",
        "kind": "LinkedField",
        "name": "productTypes",
        "plural": true,
        "selections": (v8/*:: as any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "3ca6b25115244612ced472ef3c7814ec",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationProduct_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationProduct_rootQuery(\n  $organizationCustomDomain: String!\n  $productId: String!\n  $multipleChoicesProductTagsSortingValues: [OrganizationTagOrderInput!]\n) {\n  product(id: $productId) {\n    listingMetadata {\n      title\n    }\n    id\n  }\n  ...editProduct_query\n}\n\nfragment editProduct_query on Query {\n  product(id: $productId) {\n    id\n    inactive\n    listingMetadata {\n      title\n      subTitle\n      includedFeatures\n    }\n    type {\n      type\n      name\n    }\n    currency {\n      type\n      name\n    }\n    productTags {\n      id\n      name\n      color\n    }\n    amenities {\n      id\n      name\n      color\n    }\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n      thumbnail {\n        url\n        height\n        width\n      }\n    }\n    pricingOptions {\n      id\n      index\n      listingMetadata {\n        title\n        subTitle\n      }\n      supportsSubscriptionAutoRenewal\n      purchaseCadence\n      bookingCadence\n      price\n      availableDays\n      requiredDaysPerWeek\n      numberOfResourcesToBook\n      minDurationMinutes\n      minDurationDisplayUnit\n      maxDurationMinutes\n      maxDurationDisplayUnit\n      cancellationPolicyType\n      cancellationRefundRules {\n        minutesBefore\n        displayUnit\n        refundPercentage\n      }\n      isTaxInclusive\n      maxAllowedResourcesLockTimePaidViaCard\n      maxAllowedResourcesLockTimePaidViaCardDisplayUnit\n      maxAllowedResourcesLockTimePaidViaBankTransfer\n      maxAllowedResourcesLockTimePaidViaBankTransferDisplayUnit\n      billingMode\n      acceptedPaymentMethods\n      fulfillmentType\n      entitlementCreditQuantity\n      entitlementValidityDays\n    }\n  }\n  productPricingCadences {\n    type\n    name\n  }\n  ...singleChoiceProductPricingBillingMode_query\n  currencies {\n    type\n    name\n  }\n  bookingSlotSizeInMinutes\n  defaultMaxAllowedResourcesLockTimePaidViaCard\n  defaultMaxAllowedResourcesLockTimePaidViaBankTransfer\n  durationDisplayUnits {\n    type\n    name\n  }\n  ...multipleChoicesProductTags_query\n  ...singleChoiceCurrency_query\n  ...multipleChoicesPaymentMethodTypes_query\n  ...singleChoiceProductPricingCadence_query\n  ...singleChoiceProductPricingCancellationType_query\n  ...multipleChoicesAmenities_query\n  ...singleChoiceProductType_query\n}\n\nfragment multipleChoicesAmenities_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    amenities {\n      id\n      name\n      color\n    }\n    id\n  }\n}\n\nfragment multipleChoicesPaymentMethodTypes_query on Query {\n  paymentMethodTypes {\n    type\n    name\n  }\n}\n\nfragment multipleChoicesProductTags_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    productTags(orderBy: $multipleChoicesProductTagsSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          name\n          color\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceCurrency_query on Query {\n  currencies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingBillingMode_query on Query {\n  productPricingBillingModes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingCadence_query on Query {\n  productPricingCadences {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductPricingCancellationType_query on Query {\n  productPricingCancellationTypes {\n    type\n    name\n  }\n}\n\nfragment singleChoiceProductType_query on Query {\n  productTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "1f7506cbf7a50cfcfffc7aa4968ec633";

export default node;
