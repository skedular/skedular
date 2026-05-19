/**
 * @generated SignedSource<<30d11453bee2fffeaffb0c9f550b5d49>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type observability_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"logrocket_query">;
  readonly " $fragmentType": "observability_query";
};
export type observability_query$key = {
  readonly " $data"?: observability_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"observability_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "observability_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "logrocket_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "614fe45254de776571441b157891b87c";

export default node;
