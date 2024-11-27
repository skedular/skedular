/**
 * @generated SignedSource<<1d8c60fec0bcc08816a6fecd45a8f42a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bulkNewDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesZones_query" | "multipleChoicesDeskTypes_query" | "multipleChoicesZones_query">;
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

(node as any).hash = "f8d8cc8a79528149414f81d476c1d33d";

export default node;
