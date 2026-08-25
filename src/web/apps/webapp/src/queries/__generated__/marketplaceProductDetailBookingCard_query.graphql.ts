/**
 * @generated SignedSource<<ea11ac26603877b79c1f6896f4a6f262>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceProductDetailBookingCard_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly marketplaceLocations: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly name: string;
      };
    }>;
  };
  readonly product: {
    readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailBookingCard_product">;
  } | null | undefined;
  readonly " $fragmentType": "marketplaceProductDetailBookingCard_query";
};
export type marketplaceProductDetailBookingCard_query$key = {
  readonly " $data"?: marketplaceProductDetailBookingCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceProductDetailBookingCard_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "productId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceProductDetailBookingCard_query",
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
        (v0/*:: as any*/)
      ],
      "storageKey": null
    },
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
          "name": "marketplaceProductDetailBookingCard_product"
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "fields": [
            {
              "items": [
                {
                  "kind": "Variable",
                  "name": "productIds.0",
                  "variableName": "productId"
                }
              ],
              "kind": "ListValue",
              "name": "productIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfLocationEdge",
      "kind": "LinkedField",
      "name": "marketplaceLocations",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "LocationDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "id",
                  "storageKey": null
                },
                (v0/*:: as any*/)
              ],
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
})();

(node as any).hash = "32a5f7acae78cdc73283e62fdab628fe";

export default node;
