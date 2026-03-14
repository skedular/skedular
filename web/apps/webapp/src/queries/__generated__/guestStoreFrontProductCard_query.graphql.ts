/**
 * @generated SignedSource<<9b03e56fbc02f323301695f5df6fdfbf>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontProductCard_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly productPricingCadences: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCadence;
  }>;
  readonly " $fragmentType": "guestStoreFrontProductCard_query";
};
export type guestStoreFrontProductCard_query$key = {
  readonly " $data"?: guestStoreFrontProductCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProductCard_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
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
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontProductCard_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingCadenceDetails",
      "kind": "LinkedField",
      "name": "productPricingCadences",
      "plural": true,
      "selections": (v0/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v0/*: any*/),
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "47f2395ae974a42f730d5e9265ee8db6";

export default node;
