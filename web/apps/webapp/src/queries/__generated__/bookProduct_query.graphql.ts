/**
 * @generated SignedSource<<3427c02a39954dff95bb042097103097>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookProduct_query$data = {
  readonly bookingSlotSizeInMinutes: number;
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly me: {
    readonly emails: ReadonlyArray<string>;
    readonly id: string;
  };
  readonly organization: {
    readonly taxDetails: {
      readonly taxId: string;
      readonly taxRatePercentage: any;
    } | null | undefined;
  } | null | undefined;
  readonly product: {
    readonly currency: {
      readonly name: string;
      readonly type: Currency;
    };
    readonly id: string;
    readonly latestProductVersionId: string;
    readonly listingMetadata: {
      readonly title: string | null | undefined;
    };
    readonly pricingOptions: ReadonlyArray<{
      readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
      readonly billingMode: ProductPricingBillingMode;
      readonly bookingCadence: ProductPricingCadence;
      readonly id: string;
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
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesUserEmails_query" | "singleChoiceMarketplaceBookingCategory_query" | "singleChoicePaymentMethodType_query">;
  readonly " $fragmentType": "bookProduct_query";
};
export type bookProduct_query$key = {
  readonly " $data"?: bookProduct_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookProduct_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
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
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTaxDetails",
          "kind": "LinkedField",
          "name": "taxDetails",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "taxId",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "taxRatePercentage",
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
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "listingMetadata",
          "plural": false,
          "selections": [
            (v1/*: any*/)
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
          "selections": (v2/*: any*/),
          "storageKey": null
        },
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
          "concreteType": "ProductPricing",
          "kind": "LinkedField",
          "name": "pricingOptions",
          "plural": true,
          "selections": [
            (v0/*: any*/),
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
                (v1/*: any*/),
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
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v2/*: any*/),
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
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceMarketplaceBookingCategory_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoicePaymentMethodType_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesUserEmails_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0bebd42c337c0da7728896b947d6c33f";

export default node;
