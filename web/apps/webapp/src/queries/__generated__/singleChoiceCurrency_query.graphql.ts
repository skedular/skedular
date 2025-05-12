/**
 * @generated SignedSource<<500811c5715d20639cc06e0a43560214>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceCurrency_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly " $fragmentType": "singleChoiceCurrency_query";
};
export type singleChoiceCurrency_query$key = {
  readonly " $data"?: singleChoiceCurrency_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceCurrency_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceCurrency_query",
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

(node as any).hash = "7b84aae71943412b7d84acb981b36000";

export default node;
