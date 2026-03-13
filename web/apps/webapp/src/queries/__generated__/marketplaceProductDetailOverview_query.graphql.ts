/**
 * @generated SignedSource<<5b2f4ee0272ae36194820989a6d18698>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductDetailOverview_query$data = {
  readonly product: {
    readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailOverview_product">;
  } | null | undefined;
  readonly " $fragmentType": "marketplaceProductDetailOverview_query";
};
export type marketplaceProductDetailOverview_query$key = {
  readonly " $data"?: marketplaceProductDetailOverview_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailOverview_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceProductDetailOverview_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "productId"
        }
      ],
      "concreteType": "ProductDetails",
      "kind": "LinkedField",
      "name": "product",
      "plural": false,
      "selections": [
        {
          "args": null,
          "kind": "FragmentSpread",
          "name": "marketplaceProductDetailOverview_product"
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b411a7d5d43b4e277de84ec73f9cd3dd";

export default node;
