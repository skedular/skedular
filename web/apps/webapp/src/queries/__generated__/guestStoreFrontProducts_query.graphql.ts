/**
 * @generated SignedSource<<2eaf41c7303d6e0de4fbbaa08e60fa3c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFrontProducts_query$data = {
  readonly marketplaceLocations?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly products: ReadonlyArray<{
          readonly id: string;
          readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontProductCard_product">;
        }>;
      };
    }>;
  };
  readonly products?: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
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

import guestStoreFrontProductsRefetchQuery_graphql from './guestStoreFrontProductsRefetchQuery.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  (v0/*: any*/),
  {
    "args": null,
    "kind": "FragmentSpread",
    "name": "guestStoreFrontProductCard_product"
  }
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "locationSelected"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": guestStoreFrontProductsRefetchQuery_graphql
    }
  },
  "name": "guestStoreFrontProducts_query",
  "selections": [
    {
      "condition": "locationSelected",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "fields": [
                {
                  "kind": "Variable",
                  "name": "organizationUniqueAlphanumericName",
                  "variableName": "organizationUniqueAlphanumericName"
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
                    (v0/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "ProductDetails",
                      "kind": "LinkedField",
                      "name": "products",
                      "plural": true,
                      "selections": (v1/*: any*/),
                      "storageKey": null
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
      ]
    },
    {
      "condition": "locationSelected",
      "kind": "Condition",
      "passingValue": false,
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
                      "name": "organizationUniqueAlphanumericNames.0",
                      "variableName": "organizationUniqueAlphanumericName"
                    }
                  ],
                  "kind": "ListValue",
                  "name": "organizationUniqueAlphanumericNames"
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
                  "selections": (v1/*: any*/),
                  "storageKey": null
                }
              ],
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "abcfe8c93dd9260b9f365b5485f5dfb7";

export default node;
