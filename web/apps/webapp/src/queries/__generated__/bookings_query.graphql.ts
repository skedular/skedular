/**
 * @generated SignedSource<<af12deb31f746f0e3f0962c61871b630>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookings_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentType": "bookings_query";
};
export type bookings_query$key = {
  readonly " $data"?: bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookings_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookings_query",
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

(node as any).hash = "d9e8bcf7cda8f3a086b91ac53bef6a39";

export default node;
