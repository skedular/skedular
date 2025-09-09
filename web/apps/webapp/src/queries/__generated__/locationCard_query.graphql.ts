/**
 * @generated SignedSource<<a63155a5018b241b628df089ef6afeb2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationCard_query$data = {
  readonly me: {
    readonly id: string;
    readonly preferredLocations: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly " $fragmentType": "locationCard_query";
};
export type locationCard_query$key = {
  readonly " $data"?: locationCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationCard_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationCard_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationDetails",
          "kind": "LinkedField",
          "name": "preferredLocations",
          "plural": true,
          "selections": [
            (v0/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "c318f4ff3a911657190684b31ba6ad31";

export default node;
