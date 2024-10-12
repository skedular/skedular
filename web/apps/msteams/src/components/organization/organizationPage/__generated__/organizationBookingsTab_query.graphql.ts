/**
 * @generated SignedSource<<782d4b5d874205801fe34e0cd3937f13>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBookingsTab_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query" | "newBookingDialog_query">;
  readonly " $fragmentType": "organizationBookingsTab_query";
};
export type organizationBookingsTab_query$key = {
  readonly " $data"?: organizationBookingsTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBookingsTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationBookingsTab_query",
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

(node as any).hash = "7f8f8203897f243a461bc6b3785ffaf7";

export default node;
