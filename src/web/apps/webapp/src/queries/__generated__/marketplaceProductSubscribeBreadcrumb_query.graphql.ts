/**
 * @generated SignedSource<<7bebeb33013ba458e7e8d126bb176e31>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductSubscribeBreadcrumb_query$data = {
  readonly product: {
    readonly id: string;
    readonly listingMetadata: {
      readonly title: string | null | undefined;
    };
  } | null | undefined;
  readonly " $fragmentType": "marketplaceProductSubscribeBreadcrumb_query";
};
export type marketplaceProductSubscribeBreadcrumb_query$key = {
  readonly " $data"?: marketplaceProductSubscribeBreadcrumb_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductSubscribeBreadcrumb_query">;
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
  "name": "marketplaceProductSubscribeBreadcrumb_query",
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
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        },
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

(node as any).hash = "5f117e3f177fa6a0cee3b7af49b654fe";

export default node;
