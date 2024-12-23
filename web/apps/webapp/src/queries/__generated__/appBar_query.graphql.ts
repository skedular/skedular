/**
 * @generated SignedSource<<a2ed22b58c1d01d6debae4c8c57da58a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type appBar_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"modernAppBar_query" | "oldAppBar_query">;
  readonly " $fragmentType": "appBar_query";
};
export type appBar_query$key = {
  readonly " $data"?: appBar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"appBar_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "appBar_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "modernAppBar_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "oldAppBar_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "4463166c0cab98c1407778600d16934d";

export default node;
