/**
 * @generated SignedSource<<6d6333ce1366c54b2473affdbdff7f0b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type leftSideNavigationMenu_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenuContent_query">;
  readonly " $fragmentType": "leftSideNavigationMenu_query";
};
export type leftSideNavigationMenu_query$key = {
  readonly " $data"?: leftSideNavigationMenu_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"leftSideNavigationMenu_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "leftSideNavigationMenu_query",
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

(node as any).hash = "527e21b7115309fa1c6e6a6bb7fafb50";

export default node;
