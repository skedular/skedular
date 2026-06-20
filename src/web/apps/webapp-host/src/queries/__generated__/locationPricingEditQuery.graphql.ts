/**
 * @generated SignedSource<<09a11adb36a275190981ed186c1d4b9d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
export type locationPricingEditQuery$variables = {
  locationId: string;
};
export type locationPricingEditQuery$data = {
  readonly location: {
    readonly canModify: boolean;
    readonly id: string;
    readonly products: ReadonlyArray<{
      readonly currency: {
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly url: string;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
        readonly availableDays: ReadonlyArray<DayOfWeek> | null | undefined;
        readonly billingMode: ProductPricingBillingMode;
        readonly bookingCadence: ProductPricingCadence;
        readonly cancellationPolicyType: ProductPricingCancellationPolicyType;
        readonly cancellationRefundRules: ReadonlyArray<{
          readonly minutesBefore: number;
          readonly refundPercentage: number;
        }>;
        readonly id: string;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly subTitle: string | null | undefined;
          readonly title: string | null | undefined;
        };
        readonly maxAllowedResourcesLockTimePaidViaBankTransfer: number;
        readonly maxAllowedResourcesLockTimePaidViaCard: number;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly price: any;
        readonly requiredDaysPerWeek: number | null | undefined;
        readonly supportsSubscriptionAutoRenewal: boolean;
      }>;
    }>;
  } | null | undefined;
};
export type locationPricingEditQuery = {
  response: locationPricingEditQuery$data;
  variables: locationPricingEditQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
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
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "locationId"
      }
    ],
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "location",
    "plural": false,
    "selections": [
      (v1/*:: as any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canModify",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "products",
        "plural": true,
        "selections": [
          (v1/*:: as any*/),
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
              }
            ],
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
            "concreteType": "ProductPricing",
            "kind": "LinkedField",
            "name": "pricingOptions",
            "plural": true,
            "selections": [
              (v1/*:: as any*/),
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
                "name": "bookingCadence",
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
                "name": "supportsSubscriptionAutoRenewal",
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
                "concreteType": "ListingMetadata",
                "kind": "LinkedField",
                "name": "listingMetadata",
                "plural": false,
                "selections": [
                  (v2/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "subTitle",
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
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationPricingEditQuery",
    "selections": (v3/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "locationPricingEditQuery",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "cd67ec12a20d63b1f91fb9249348f449",
    "id": null,
    "metadata": {},
    "name": "locationPricingEditQuery",
    "operationKind": "query",
    "text": "query locationPricingEditQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    id\n    canModify\n    products {\n      id\n      currency {\n        type\n      }\n      listingMetadata {\n        title\n        about\n      }\n      featureImages {\n        original {\n          url\n        }\n      }\n      pricingOptions {\n        id\n        price\n        bookingCadence\n        billingMode\n        acceptedPaymentMethods\n        availableDays\n        requiredDaysPerWeek\n        minDurationMinutes\n        maxDurationMinutes\n        cancellationPolicyType\n        cancellationRefundRules {\n          minutesBefore\n          refundPercentage\n        }\n        isTaxInclusive\n        supportsSubscriptionAutoRenewal\n        maxAllowedResourcesLockTimePaidViaCard\n        maxAllowedResourcesLockTimePaidViaBankTransfer\n        listingMetadata {\n          title\n          subTitle\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7130b7e9d7fd987a80695d4fbaf71c74";

export default node;
