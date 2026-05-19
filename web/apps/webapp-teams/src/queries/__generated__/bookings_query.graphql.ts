/**
 * @generated SignedSource<<de72ce432349510aba335b6e99117a8f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookings_query$data = {
  readonly me: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query">;
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "3b239104345c65c2c0baa8aba64f2b80";

export default node;
