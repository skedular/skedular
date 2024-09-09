/**
 * @generated SignedSource<<216d9e692a7e54288f58687208e3d953>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesZones_query">;
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "89f0d5a23e0c70feb3a694802a8349fd";

export default node;
