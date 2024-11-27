/**
 * @generated SignedSource<<30a15cd3049b8e07aa407278b7316b32>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bulkNewDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesZones_query" | "multipleChoicesDeskTypes_query">;
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
      "name": "multipleChoicesDeskTypes_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "cbd9968093c210477c62658b822b961e";

export default node;
