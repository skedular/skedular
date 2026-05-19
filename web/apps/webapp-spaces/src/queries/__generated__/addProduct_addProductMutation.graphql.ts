/**
 * @generated SignedSource<<7732061af696d196f414100a8aa2d846>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
export type ProductType = "EVENT" | "RESOURCE" | "%future added value";
export type AddProductInput = {
  clientMutationId?: string | null | undefined;
  currency: Currency;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  listingMetadata?: ListingMetadataInput | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  organizationId?: string | null | undefined;
  pricingOptions: ReadonlyArray<ProductPricingInput>;
  tagIds: ReadonlyArray<string>;
  type: ProductType;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type ListingMetadataInput = {
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type ProductPricingInput = {
  acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
  billingMode: ProductPricingBillingMode;
  bookingCadence: ProductPricingCadence;
  cancellationPolicyType: ProductPricingCancellationPolicyType;
  cancellationRefundRules: ReadonlyArray<ProductPricingCancellationRefundRuleInput>;
  id: string;
  index: number;
  isTaxInclusive: boolean;
  listingMetadata: ListingMetadataInput;
  maxAllowedResourcesLockTimePaidViaBankTransfer: number;
  maxAllowedResourcesLockTimePaidViaCard: number;
  maxDurationMinutes?: number | null | undefined;
  minDurationMinutes?: number | null | undefined;
  numberOfResourcesToBook: number;
  price: any;
  purchaseCadence: ProductPricingCadence;
  supportsSubscriptionAutoRenewal: boolean;
};
export type ProductPricingCancellationRefundRuleInput = {
  minutesBefore: number;
  refundPercentage: number;
};
export type addProduct_addProductMutation$variables = {
  input: AddProductInput;
};
export type addProduct_addProductMutation$data = {
  readonly addProduct: {
    readonly product: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
        readonly billingMode: ProductPricingBillingMode;
        readonly bookingCadence: ProductPricingCadence;
        readonly cancellationPolicyType: ProductPricingCancellationPolicyType;
        readonly cancellationRefundRules: ReadonlyArray<{
          readonly minutesBefore: number;
          readonly refundPercentage: number;
        }>;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
        readonly maxAllowedResourcesLockTimePaidViaCard: number;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly numberOfResourcesToBook: number;
        readonly price: any;
        readonly purchaseCadence: ProductPricingCadence;
        readonly supportsSubscriptionAutoRenewal: boolean;
      }>;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly type: {
        readonly name: string;
        readonly type: ProductType;
      };
    };
  };
};
export type addProduct_addProductMutation$rawResponse = {
  readonly addProduct: {
    readonly product: {
      readonly amenities: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly name: string;
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
        readonly thumbnail: {
          readonly height: number | null | undefined;
          readonly url: string;
          readonly width: number | null | undefined;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly inactive: boolean;
      readonly listingMetadata: {
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
        readonly billingMode: ProductPricingBillingMode;
        readonly bookingCadence: ProductPricingCadence;
        readonly cancellationPolicyType: ProductPricingCancellationPolicyType;
        readonly cancellationRefundRules: ReadonlyArray<{
          readonly minutesBefore: number;
          readonly refundPercentage: number;
        }>;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
        readonly maxAllowedResourcesLockTimePaidViaCard: number;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly numberOfResourcesToBook: number;
        readonly price: any;
        readonly purchaseCadence: ProductPricingCadence;
        readonly supportsSubscriptionAutoRenewal: boolean;
      }>;
      readonly productTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly id: string;
        readonly name: string;
      }>;
      readonly type: {
        readonly name: string;
        readonly type: ProductType;
      };
    };
  };
};
export type addProduct_addProductMutation = {
  rawResponse: addProduct_addProductMutation$rawResponse;
  response: addProduct_addProductMutation$data;
  variables: addProduct_addProductMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v4/*:: as any*/)
],
v6 = [
  (v1/*:: as any*/),
  (v4/*:: as any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v7 = [
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
v8 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "ProductPayload",
    "kind": "LinkedField",
    "name": "addProduct",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
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
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": [
              (v2/*:: as any*/),
              (v3/*:: as any*/),
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
            "concreteType": "ProductTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v5/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CurrencyDetails",
            "kind": "LinkedField",
            "name": "currency",
            "plural": false,
            "selections": (v5/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "productTags",
            "plural": true,
            "selections": (v6/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationTagDetails",
            "kind": "LinkedField",
            "name": "amenities",
            "plural": true,
            "selections": (v6/*:: as any*/),
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
                "selections": (v7/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v7/*:: as any*/),
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
                  (v2/*:: as any*/),
                  (v3/*:: as any*/)
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
                "name": "maxAllowedResourcesLockTimePaidViaBankTransfer",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addProduct_addProductMutation",
    "selections": (v8/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addProduct_addProductMutation",
    "selections": (v8/*:: as any*/)
  },
  "params": {
    "cacheID": "9f69833eeb4f141b57c8fdc93322a162",
    "id": null,
    "metadata": {},
    "name": "addProduct_addProductMutation",
    "operationKind": "mutation",
    "text": "mutation addProduct_addProductMutation(\n  $input: AddProductInput!\n) {\n  addProduct(input: $input) {\n    product {\n      id\n      inactive\n      listingMetadata {\n        title\n        subTitle\n        includedFeatures\n      }\n      type {\n        type\n        name\n      }\n      currency {\n        type\n        name\n      }\n      productTags {\n        id\n        name\n        color\n      }\n      amenities {\n        id\n        name\n        color\n      }\n      featureImages {\n        original {\n          url\n          height\n          width\n        }\n        thumbnail {\n          url\n          height\n          width\n        }\n      }\n      pricingOptions {\n        index\n        listingMetadata {\n          title\n          subTitle\n        }\n        supportsSubscriptionAutoRenewal\n        purchaseCadence\n        bookingCadence\n        price\n        numberOfResourcesToBook\n        minDurationMinutes\n        maxDurationMinutes\n        cancellationPolicyType\n        cancellationRefundRules {\n          minutesBefore\n          refundPercentage\n        }\n        isTaxInclusive\n        maxAllowedResourcesLockTimePaidViaCard\n        maxAllowedResourcesLockTimePaidViaBankTransfer\n        billingMode\n        acceptedPaymentMethods\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "db2cc2648210dcceb0029406c5a947d3";

export default node;
