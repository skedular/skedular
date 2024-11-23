/**
 * @generated SignedSource<<23877098e8bf1a5f98766a7772b75fb3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesDeskTypes_query" | "deskMultipleChoicesZones_query">;
  readonly " $fragmentType": "newDeskDialog_query";
};
export type newDeskDialog_query$key = {
  readonly " $data"?: newDeskDialog_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"newDeskDialog_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "newDeskDialog_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "deskMultipleChoicesZones_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "deskMultipleChoicesDeskTypes_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0273c659c87d20679067cacc443ded4d";

export default node;
