/**
 * @generated SignedSource<<56c1c1f8179852db04729fd6e7d9c113>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "Nzd" | "Usd" | "%future added value";
export type PriceUnit = "PerHour" | "PerMinute" | "PerUse" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type productCard_ProductDetails$data = {
  readonly bookAllLocationResources: boolean;
  readonly currency: {
    readonly name: string;
    readonly type: Currency;
  };
  readonly description: string | null | undefined;
  readonly forceContinuousSlots: boolean;
  readonly id: string;
  readonly inactive: boolean;
  readonly maxDurationMinutes: number | null | undefined;
  readonly maxSpreadDays: number | null | undefined;
  readonly minDurationMinutes: number | null | undefined;
  readonly name: string;
  readonly organization: {
    readonly uniqueId: string;
  };
  readonly price: string;
  readonly priceUnit: {
    readonly name: string;
    readonly type: PriceUnit;
  };
  readonly recurrenceIntervalDays: number;
  readonly " $fragmentType": "productCard_ProductDetails";
};
export type productCard_ProductDetails$key = {
  readonly " $data"?: productCard_ProductDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"productCard_ProductDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v0/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "productCard_ProductDetails",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "id",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
    (v0/*: any*/),
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
      "selections": (v1/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currency",
      "plural": false,
      "selections": (v1/*: any*/),
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
      "name": "recurrenceIntervalDays",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "forceContinuousSlots",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "maxSpreadDays",
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
          "name": "uniqueId",
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

(node as any).hash = "a5cecdd362fd83041e9cbb1bd1b0041e";

export default node;
