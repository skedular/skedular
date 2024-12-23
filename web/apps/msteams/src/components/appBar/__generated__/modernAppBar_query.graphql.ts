/**
 * @generated SignedSource<<f7089c369ae693eb0ebd453fbf6de437>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type modernAppBar_query$data = {
  readonly me: {
    readonly email: string | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"newFeedbackDialog_query">;
  readonly " $fragmentType": "modernAppBar_query";
};
export type modernAppBar_query$key = {
  readonly " $data"?: modernAppBar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"modernAppBar_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "modernAppBar_query",
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
          "name": "email",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "givenName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "middleName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "familyName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "photoUrl",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "newFeedbackDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "f80b65d430079c27fd831599634a4c62";

export default node;
