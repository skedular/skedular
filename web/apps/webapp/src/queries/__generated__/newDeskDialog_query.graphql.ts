/**
 * @generated SignedSource<<6d6f4b642a3acbee7800099733540cbe>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newDeskDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"deskMultipleChoicesZones_query" | "multipleChoicesDeskTypes_query" | "multipleChoicesZones_query">;
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

(node as any).hash = "58f5bdd22f4869879f27f80b34b3a997";

export default node;
