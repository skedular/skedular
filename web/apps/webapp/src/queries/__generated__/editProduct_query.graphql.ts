/**
 * @generated SignedSource<<d2e2b65fb56c3ec85414bbc18f46faf6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingCadence = "DAILY_V1" | "FIVE_MONTHS_V1" | "FOUR_MONTHS_V1" | "HALF_DAY_V1" | "MONTHLY_V1" | "NOT_SET" | "ONE_TIME_V1" | "PER15_MINUTE_V1" | "PER30_MINUTE_V1" | "PER_HOUR_V1" | "PER_MINUTE_V1" | "QUARTERLY_V1" | "SIX_MONTHS_V1" | "TWO_MONTHS_V1" | "WEEKLY_V1" | "YEARLY_V1" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editProduct_query$data = {
  readonly bookingSlotSizeInMinutes: number;
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly defaultMaxAllowedResourcesLockTimePaidViaBankTransfer: number;
  readonly defaultMaxAllowedResourcesLockTimePaidViaCard: number;
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
    readonly organization: {
      readonly id: string;
    };
    readonly pricingOptions: ReadonlyArray<{
      readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
      readonly cadence: ProductPricingCadence;
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
    }>;
    readonly productTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly id: string;
      readonly name: string;
    }>;
  } | null | undefined;
  readonly productPricingCadences: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCadence;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesAmenities_query" | "multipleChoicesBookingPaymentMethodTypes_query" | "multipleChoicesProductTags_query" | "singleChoiceCurrency_query" | "singleChoiceProductPricingCadence_query">;
  readonly " $fragmentType": "editProduct_query";
};
export type editProduct_query$key = {
  readonly " $data"?: editProduct_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editProduct_query">;
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*: any*/)
],
v5 = [
  (v0/*: any*/),
  (v3/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v6 = [
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
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editProduct_query",
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
        (v0/*: any*/),
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
            (v1/*: any*/),
            (v2/*: any*/),
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
          "concreteType": "CurrencyDetails",
          "kind": "LinkedField",
          "name": "currency",
          "plural": false,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "productTags",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "amenities",
          "plural": true,
          "selections": (v5/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            (v0/*: any*/)
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
              "selections": (v6/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "thumbnail",
              "plural": false,
              "selections": (v6/*: any*/),
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
                (v1/*: any*/),
                (v2/*: any*/)
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
      "concreteType": "ProductPricingCadenceDetails",
      "kind": "LinkedField",
      "name": "productPricingCadences",
      "plural": true,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v4/*: any*/),
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
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesProductTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceCurrency_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesBookingPaymentMethodTypes_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceProductPricingCadence_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesAmenities_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "4deb5a28350d0fca9b591d05a2943bfe";

export default node;
