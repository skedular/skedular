/**
 * @generated SignedSource<<d1fd68fa320861b2463502e6cd3a0254>>
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
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query" | "newBookingDialog_query">;
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "newBookingDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b1cb96661a32ca79ec5f32c6d388dfe5";

export default node;
