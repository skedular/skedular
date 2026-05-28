/**
 * @generated SignedSource<<3c8996d2ebcc06e359f520274596c7b6>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type weekOpeningHours_query$data = {
  readonly bookingSlotSizeInMinutes: number;
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
      "name": "bookingSlotSizeInMinutes",
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "d7b612aeb6b6fb0daf893d740ddc63cb";

export default node;
