/**
 * @generated SignedSource<<fff30b73aaf9563873cb5cb93af4f2bd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type PriceUnit = "PER_HOUR" | "PER_MINUTE" | "PER_USE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookProduct_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly openingHoursMinutesStep: number;
  readonly product: {
    readonly bookAllLocationResources: boolean;
    readonly currency: {
      readonly name: string;
      readonly type: Currency;
    };
    readonly currencyToDisplay: string;
    readonly description: string | null | undefined;
    readonly id: string;
    readonly latestProductVersionId: string;
    readonly maxBookingSpreadDays: number | null | undefined;
    readonly maxDurationMinutes: number | null | undefined;
    readonly minDurationMinutes: number | null | undefined;
    readonly name: string;
    readonly numberOfResourcesToBook: number;
    readonly price: string;
    readonly priceUnit: {
      readonly name: string;
      readonly type: PriceUnit;
    };
    readonly recurrenceWindowDays: number;
    readonly requireConsecutiveDays: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMarketplaceBookingType_query">;
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
  (v1/*: any*/)
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
        (v0/*: any*/)
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
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "description",
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
          "concreteType": "PriceUnitDetails",
          "kind": "LinkedField",
          "name": "priceUnit",
          "plural": false,
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "currencyToDisplay",
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
          "name": "bookAllLocationResources",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "recurrenceWindowDays",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "requireConsecutiveDays",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "maxBookingSpreadDays",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "latestProductVersionId",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceMarketplaceBookingType_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "3c24ae4fca6ecfbef8a1d5ab1d0b0019";

export default node;
