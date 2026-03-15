/**
 * @generated SignedSource<<5612f30d481c2bfccec1dddfd7dfc537>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductDetailBookingCard_product$data = {
  readonly amenities: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
  readonly currency: {
    readonly name: string;
    readonly type: Currency;
  };
  readonly id: string;
  readonly listingMetadata: {
    readonly about: string | null | undefined;
    readonly includedFeatures: ReadonlyArray<string> | null | undefined;
    readonly subTitle: string | null | undefined;
    readonly title: string | null | undefined;
  };
  readonly pricingOptions: ReadonlyArray<{
    readonly acceptedPaymentMethods: ReadonlyArray<PaymentMethod>;
    readonly id: string;
    readonly index: number;
    readonly isTaxInclusive: boolean;
    readonly listingMetadata: {
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly maxDurationMinutes: number | null | undefined;
    readonly minDurationMinutes: number | null | undefined;
    readonly numberOfResourcesToBook: number;
    readonly price: any;
    readonly purchaseCadence: ProductPricingCadence;
    readonly supportsSubscriptionAutoRenewal: boolean;
  }>;
  readonly " $fragmentType": "marketplaceProductDetailBookingCard_product";
};
export type marketplaceProductDetailBookingCard_product$key = {
  readonly " $data"?: marketplaceProductDetailBookingCard_product$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailBookingCard_product">;
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
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceProductDetailBookingCard_product",
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
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "about",
          "storageKey": null
        },
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
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "amenities",
      "plural": true,
      "selections": [
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
        (v3/*: any*/)
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
            (v2/*: any*/)
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
  "type": "ProductDetails",
  "abstractKey": null
};
})();

(node as any).hash = "c393bc342f42ee03d00ce74af3e37ae6";

export default node;
