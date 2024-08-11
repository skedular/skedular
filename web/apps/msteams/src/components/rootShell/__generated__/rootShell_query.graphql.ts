/**
 * @generated SignedSource<<294d8c27d558f3017cd667df4ef84043>>
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
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly tenantInstalled: boolean;
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
      "name": "tenantInstalled",
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

(node as any).hash = "a677733fc4c62455bc1aefc5faf251a3";

export default node;
