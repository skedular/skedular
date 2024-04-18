/**
 * @generated SignedSource<<9b6afb5cd56ddfbdee88a4b13fea9596>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type rootShell_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"mainRootLayout_query" | "observability_query">;
  readonly " $fragmentType": "rootShell_query";
};
export type rootShell_query$key = {
  readonly " $data"?: rootShell_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "rootShell_query",
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "observability_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "mainRootLayout_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "7709a9d4696523b42de4c356602ce4cb";

export default node;
