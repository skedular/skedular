/**
 * @generated SignedSource<<e550cfac0007343750b880e200c2e6a5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bulkNewDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesDeskTypes_query" | "deskMultipleChoicesZones_query">;
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

(node as any).hash = "c057aa7dd28cbf3b9b50df30af88d5ad";

export default node;
