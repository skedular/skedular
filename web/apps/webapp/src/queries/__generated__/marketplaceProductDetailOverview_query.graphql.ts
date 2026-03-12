/**
 * @generated SignedSource<<688c24890205c81650143e2e48dda8ac>>
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
    readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailSharedProductFragment_product">;
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
          "name": "marketplaceProductDetailSharedProductFragment_product"
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b68cbbfb0e225e2446883ea41dcc65b3";

export default node;
