/**
 * @generated SignedSource<<92443a3d04e7b53071811c9f43aac967>>
 * @lightSyntaxTransform
 * @nogrep
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
