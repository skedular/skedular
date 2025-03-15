/**
 * @generated SignedSource<<fe932aaada9cd3623582d884aa9e4a6e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type weekOpeningHours_query$data = {
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentType": "weekOpeningHours_query";
};
export type weekOpeningHours_query$key = {
  readonly " $data"?: weekOpeningHours_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"weekOpeningHours_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "weekOpeningHours_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6acd92c93544bbc76455b92dab2b54ee";

export default node;
