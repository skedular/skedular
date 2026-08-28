/**
 * @generated SignedSource<<f793b02e7469cf673a413c0f107c1f5f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductDetailBreadcrumb_query$data = {
  readonly product: {
    readonly listingMetadata: {
      readonly title: string | null | undefined;
    };
  } | null | undefined;
  readonly " $fragmentType": "marketplaceProductDetailBreadcrumb_query";
};
export type marketplaceProductDetailBreadcrumb_query$key = {
  readonly " $data"?: marketplaceProductDetailBreadcrumb_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailBreadcrumb_query">;
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
  "name": "marketplaceProductDetailBreadcrumb_query",
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
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "listingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "title",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "d2877f3a1f8706eefa31a2f31c1ade08";

export default node;
