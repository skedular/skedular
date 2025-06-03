/**
 * @generated SignedSource<<cfcca6d12d80d1bbc8fa2aa31a66116a>>
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
  readonly id: string;
  readonly maxBookingSpreadDays: number | null | undefined;
  readonly maxDurationMinutes: number | null | undefined;
  readonly minDurationMinutes: number | null | undefined;
  readonly name: string;
  readonly numberOfResourcesToBook: number;
  readonly organization: {
    readonly uniqueId: string;
  };
  readonly priceToDisplay: string;
  readonly priceUnit: {
    readonly name: string;
  };
  readonly primaryFeatureImageUrl: string | null | undefined;
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
  "name": "name",
  "storageKey": null
};
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
        (v0/*: any*/)
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "primaryFeatureImageUrl",
      "storageKey": null
    }
  ],
  "type": "ProductDetails",
  "abstractKey": null
};
})();

(node as any).hash = "56f9c50935293155c0ba5ac3e2d7b46c";

export default node;
