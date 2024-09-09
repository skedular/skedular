/**
 * @generated SignedSource<<6b96a481cdf1cd1a4dfe525d3431bc22>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type mainRootLayout_query$data = {
  readonly me: {
    readonly email: {
      readonly email: string;
      readonly verified: boolean;
    } | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly middleName: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"newFeedbackDialog_query">;
  readonly " $fragmentType": "mainRootLayout_query";
};
export type mainRootLayout_query$key = {
  readonly " $data"?: mainRootLayout_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"mainRootLayout_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "mainRootLayout_query",
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
          "concreteType": "CustomerEmail",
          "kind": "LinkedField",
          "name": "email",
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
              "name": "verified",
              "storageKey": null
            }
          ],
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

(node as any).hash = "fe1c9119227d6385b98ba8dcc8931d1d";

export default node;
