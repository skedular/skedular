/**
 * @generated SignedSource<<db6415aa3547c76dc1846b12971f8666>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductBookingBreadcrumb_query$data = {
  readonly product: {
    readonly id: string;
    readonly listingMetadata: {
      readonly title: string | null | undefined;
    };
  } | null | undefined;
  readonly " $fragmentType": "marketplaceProductBookingBreadcrumb_query";
};
export type marketplaceProductBookingBreadcrumb_query$key = {
  readonly " $data"?: marketplaceProductBookingBreadcrumb_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductBookingBreadcrumb_query">;
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
  "name": "marketplaceProductBookingBreadcrumb_query",
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

(node as any).hash = "f61cf07873817281f21c606bc4d56662";

export default node;
