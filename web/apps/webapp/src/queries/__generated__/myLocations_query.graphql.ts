/**
 * @generated SignedSource<<e2dac6c6ad6333cd4a2f846ef860390e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myLocations_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "myLocations_query";
};
export type myLocations_query$key = {
  readonly " $data"?: myLocations_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myLocations_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myLocations_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "0ceaf4bdbfc219b8e11223a12e9e32fe";

export default node;
