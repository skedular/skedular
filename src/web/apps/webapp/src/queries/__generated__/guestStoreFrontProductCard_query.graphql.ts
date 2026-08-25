/**
 * @generated SignedSource<<dfdbf17f830a1593472256a23fd2556f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontProductCard_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly " $fragmentType": "guestStoreFrontProductCard_query";
};
export type guestStoreFrontProductCard_query$key = {
  readonly " $data"?: guestStoreFrontProductCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProductCard_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontProductCard_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "adf92838e0bb1b319f5dd829c405b3c9";

export default node;
