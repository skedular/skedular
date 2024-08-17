/**
 * @generated SignedSource<<ff74cd02c3dfae61a776def9c3426996>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type rootShell_query$data = {
  readonly adminConsentUrl: string;
  readonly isMsTeamsTenantInstalled: boolean;
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
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isMsTeamsTenantInstalled",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "adminConsentUrl",
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

(node as any).hash = "77498d225074ff864bcd2ba36782bd52";

export default node;
