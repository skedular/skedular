/**
 * @generated SignedSource<<17f790df1457ca4f457efd6af2aa08ad>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type mobileLeftSideNavigationMenu_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenuContent_query">;
  readonly " $fragmentType": "mobileLeftSideNavigationMenu_query";
};
export type mobileLeftSideNavigationMenu_query$key = {
  readonly " $data"?: mobileLeftSideNavigationMenu_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"mobileLeftSideNavigationMenu_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "mobileLeftSideNavigationMenu_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "leftSideNavigationMenuContent_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "9f793dfa659a4d9e9d296523c98ba9e8";

export default node;
