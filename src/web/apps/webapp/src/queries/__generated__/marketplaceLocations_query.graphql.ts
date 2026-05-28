/**
 * @generated SignedSource<<bae9990bd7f398213d59a8d97ee40eac>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocations_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocationCard_query">;
  readonly " $fragmentType": "marketplaceLocations_query";
};
export type marketplaceLocations_query$key = {
  readonly " $data"?: marketplaceLocations_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocations_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceLocations_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "marketplaceLocationCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "00f6ff426072bdc75753b43347b816e4";

export default node;
