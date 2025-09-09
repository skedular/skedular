/**
 * @generated SignedSource<<82994ae712c965f86668f1861364e64c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamCard_query$data = {
  readonly me: {
    readonly id: string;
    readonly preferredTeams: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly " $fragmentType": "teamCard_query";
};
export type teamCard_query$key = {
  readonly " $data"?: teamCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamCard_query">;
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
  "name": "teamCard_query",
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
          "concreteType": "TeamDetails",
          "kind": "LinkedField",
          "name": "preferredTeams",
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

(node as any).hash = "e5f13fbbdbe7647dff3d7ff49d85d73b";

export default node;
