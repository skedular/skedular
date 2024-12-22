/**
 * @generated SignedSource<<c3326dde0680abf30d15e35e68fae873>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type leftSideNavigationMenuContent_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"modernLeftSideNavigationMenuContent_query">;
  readonly " $fragmentType": "leftSideNavigationMenuContent_query";
};
export type leftSideNavigationMenuContent_query$key = {
  readonly " $data"?: leftSideNavigationMenuContent_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenuContent_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "leftSideNavigationMenuContent_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "modernLeftSideNavigationMenuContent_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "66f730e93a37de395480280fae825eea";

export default node;
