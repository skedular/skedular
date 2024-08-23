/**
 * @generated SignedSource<<a9ddc841562ceee62b2938c77bb9c6fc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type rootShell_query$data = {
  readonly azureTenantAdminConsentUrl: string;
  readonly isAzureTenantInstalled: boolean;
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
      "name": "isAzureTenantInstalled",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "azureTenantAdminConsentUrl",
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

(node as any).hash = "f47c9f6cded11baf3162ab3aba3a2426";

export default node;
