/**
 * @generated SignedSource<<97244612f358cd0a5992ca770bfe6312>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBookings_query$data = {
  readonly me: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"myBookingCard_query">;
  readonly " $fragmentType": "myBookings_query";
};
export type myBookings_query$key = {
  readonly " $data"?: myBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookings_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myBookings_query",
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
      "name": "myBookingCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "292cf485a75cb941443cfb90336e4785";

export default node;
