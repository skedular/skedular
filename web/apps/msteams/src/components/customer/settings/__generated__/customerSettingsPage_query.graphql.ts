/**
 * @generated SignedSource<<652da6575348321c9fed0c9509afc644>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customerSettingsPage_query$data = {
  readonly me: {
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"customerSettingsPersonalTab_query">;
  readonly " $fragmentType": "customerSettingsPage_query";
};
export type customerSettingsPage_query$key = {
  readonly " $data"?: customerSettingsPage_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"customerSettingsPage_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "customerSettingsPage_query",
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
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
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
      "name": "customerSettingsPersonalTab_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "1ccab5222905cfdc26006def84602ba0";

export default node;
