/**
 * @generated SignedSource<<871d36601f8a5e860fe573bf6dda1274>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesDeskTypes_query" | "multipleChoicesZones_query">;
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
      "name": "multipleChoicesDeskTypes_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesZones_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "f8064338cc807c9ed13b9a7f4b5acaf9";

export default node;
