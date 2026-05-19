/**
 * @generated SignedSource<<879a861986205933ee2171f13aa1e18d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontProducts_query$data = {
  readonly products: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly pricingOptions: ReadonlyArray<{
          readonly index: number;
        }>;
        readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProductCard_product">;
      };
    }>;
  };
  readonly " $fragmentType": "guestStoreFrontProducts_query";
};
export type guestStoreFrontProducts_query$key = {
  readonly " $data"?: guestStoreFrontProducts_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProducts_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "guestStoreFrontProducts_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "fields": [
            {
              "kind": "Literal",
              "name": "includeInactive",
              "value": false
            },
            {
              "items": [
                {
                  "kind": "Variable",
                  "name": "organizationCustomDomains.0",
                  "variableName": "organizationCustomDomain"
                }
              ],
              "kind": "ListValue",
              "name": "organizationCustomDomains"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfProductEdge",
      "kind": "LinkedField",
      "name": "products",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "ProductEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "ProductDetails",
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
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ProductPricing",
                  "kind": "LinkedField",
                  "name": "pricingOptions",
                  "plural": true,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "index",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "guestStoreFrontProductCard_product"
                }
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

(node as any).hash = "9e186197fa0d3967e503da2d4d1abf92";

export default node;
