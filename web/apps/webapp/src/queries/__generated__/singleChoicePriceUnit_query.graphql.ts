/**
 * @generated SignedSource<<aba914b6a280a877c5a42612228d7c76>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PriceUnit = "PerHour" | "PerMinute" | "PerUse" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoicePriceUnit_query$data = {
  readonly priceUnits: ReadonlyArray<{
    readonly name: string;
    readonly type: PriceUnit;
  }>;
  readonly " $fragmentType": "singleChoicePriceUnit_query";
};
export type singleChoicePriceUnit_query$key = {
  readonly " $data"?: singleChoicePriceUnit_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoicePriceUnit_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoicePriceUnit_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PriceUnitDetails",
      "kind": "LinkedField",
      "name": "priceUnits",
      "plural": true,
      "selections": [
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
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "14ec12a219f633d74b5a62b887dd4e49";

export default node;
