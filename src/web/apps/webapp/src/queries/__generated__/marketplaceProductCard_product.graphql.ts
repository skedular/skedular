/**
 * @generated SignedSource<<eb541753502b4bc0ab183a64a6cc8613>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type DayOfWeek = "FRIDAY" | "MONDAY" | "SATURDAY" | "SUNDAY" | "THURSDAY" | "TUESDAY" | "WEDNESDAY" | "%future added value";
export type MembershipTerm = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "MONTHLY" | "NOT_SET" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductCard_product$data = {
  readonly currency: {
    readonly name: string;
  };
  readonly featureImages: ReadonlyArray<{
    readonly original: {
      readonly url: string;
    } | null | undefined;
  }>;
  readonly id: string;
  readonly listingMetadata: {
    readonly subTitle: string | null | undefined;
    readonly title: string | null | undefined;
  };
  readonly pricingOptions: ReadonlyArray<{
    readonly availableDays: ReadonlyArray<DayOfWeek> | null | undefined;
    readonly id: string;
    readonly index: number;
    readonly isTaxInclusive: boolean;
    readonly listingMetadata: {
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly membershipTerm: MembershipTerm;
    readonly price: any;
  }>;
  readonly " $fragmentType": "marketplaceProductCard_product";
};
export type marketplaceProductCard_product$key = {
  readonly " $data"?: marketplaceProductCard_product$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductCard_product">;
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
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceProductCard_product",
  "selections": [
    (v0/*:: as any*/),
    (v1/*:: as any*/),
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
          "name": "name",
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
        (v0/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "index",
          "storageKey": null
        },
        (v1/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "membershipTerm",
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
          "name": "availableDays",
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

(node as any).hash = "a8bd1f5db01fdc7815b898e811c6395d";

export default node;
