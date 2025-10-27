/**
 * @generated SignedSource<<9ab692573051e38a431ccfb453b23688>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocations_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"resourceTypeSelector_allResourceTypes_query">;
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
      "name": "resourceTypeSelector_allResourceTypes_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "d6e87bf646b9479c5b1c5c5a8b21faee";

export default node;
