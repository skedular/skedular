/**
 * @generated SignedSource<<56851f7b151f768811c52a1059626768>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type productCard_ProductDetails$data = {
  readonly description: string | null | undefined;
  readonly featureImages: ReadonlyArray<{
    readonly thumbnail: {
      readonly height: number | null | undefined;
      readonly url: string;
      readonly width: number | null | undefined;
    } | null | undefined;
  }>;
  readonly id: string;
  readonly inactive: boolean;
  readonly isPriceTaxInclusive: boolean;
  readonly maxBookingSpreadDays: number | null | undefined;
  readonly maxDurationMinutes: number | null | undefined;
  readonly minDurationMinutes: number | null | undefined;
  readonly name: string;
  readonly numberOfResourcesToBook: number;
  readonly organization: {
    readonly id: string;
  };
  readonly priceToDisplay: string;
  readonly priceUnit: {
    readonly name: string;
  };
  readonly requireConsecutiveDays: boolean;
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
  "name": "id",
  "storageKey": null
},
v1 = {
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
  "name": "productCard_ProductDetails",
  "selections": [
    (v0/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
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
      "name": "priceToDisplay",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PriceUnitDetails",
      "kind": "LinkedField",
      "name": "priceUnit",
      "plural": false,
      "selections": [
        (v1/*: any*/)
      ],
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
          "name": "thumbnail",
          "plural": false,
          "selections": [
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
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isPriceTaxInclusive",
      "storageKey": null
    }
  ],
  "type": "ProductDetails",
  "abstractKey": null
};
})();

(node as any).hash = "647481268230ba6c139d7b20cfc4d437";

export default node;
