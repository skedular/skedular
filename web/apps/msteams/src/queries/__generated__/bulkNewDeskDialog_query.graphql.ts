/**
 * @generated SignedSource<<6346838acff6bc60ea74c1e9fc95981c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bulkNewDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesZones_query">;
  readonly " $fragmentType": "bulkNewDeskDialog_query";
};
export type bulkNewDeskDialog_query$key = {
  readonly " $data"?: bulkNewDeskDialog_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bulkNewDeskDialog_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "bulkNewDeskDialog_query",
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

(node as any).hash = "42a7eaeb076e59048c9c60ce7b83840c";

export default node;
