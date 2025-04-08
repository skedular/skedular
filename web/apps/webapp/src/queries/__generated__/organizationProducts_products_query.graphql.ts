/**
 * @generated SignedSource<<c20d9411bf7f21fa3ee5f265c43aaac6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "Nzd" | "Usd" | "%future added value";
export type PriceUnit = "PerHour" | "PerMinute" | "PerUse" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationProducts_products_query$data = {
  readonly products: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookAllLocationResources: boolean;
        readonly currency: {
          readonly name: string;
          readonly type: Currency;
        };
        readonly description: string | null | undefined;
        readonly id: string;
        readonly inactive: boolean;
        readonly maxBookingSpreadDays: number | null | undefined;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly name: string;
        readonly numberOfResourcesToBook: number;
        readonly organization: {
          readonly uniqueId: string;
        };
        readonly price: string;
        readonly priceUnit: {
          readonly name: string;
          readonly type: PriceUnit;
        };
        readonly recurrenceWindowDays: number;
        readonly requireConsecutiveDays: boolean;
        readonly " $fragmentSpreads": FragmentRefs<"productCard_ProductDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "organizationProducts_products_query";
};
export type organizationProducts_products_query$key = {
  readonly " $data"?: organizationProducts_products_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationProducts_products_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "products"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "count"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "cursor"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "productsSortingValues"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*: any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*: any*/)
      },
      "fragmentPathInResult": [],
      "operation": require('./organizationProducts_products_refetchableFragment.graphql')
    }
  },
  "name": "organizationProducts_products_query",
  "selections": [
    {
      "alias": "products",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "productsSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Literal",
              "name": "includeInactive",
              "value": true
            },
            {
              "items": [
                {
                  "kind": "Variable",
                  "name": "organizationIds.0",
                  "variableName": "organizationId"
                }
              ],
              "kind": "ListValue",
              "name": "organizationIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ProductConnection",
      "kind": "LinkedField",
      "name": "__organizationProducts_products_connection",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalCount",
          "storageKey": null
        },
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
                  "kind": "ScalarField",
                  "name": "inactive",
                  "storageKey": null
                },
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "description",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "price",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "PriceUnitDetails",
                  "kind": "LinkedField",
                  "name": "priceUnit",
                  "plural": false,
                  "selections": (v2/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CurrencyDetails",
                  "kind": "LinkedField",
                  "name": "currency",
                  "plural": false,
                  "selections": (v2/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "numberOfResourcesToBook",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "minDurationMinutes",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "maxDurationMinutes",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "bookAllLocationResources",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "recurrenceWindowDays",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "requireConsecutiveDays",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "maxBookingSpreadDays",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Marketplace_OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "uniqueId",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "productCard_ProductDetails"
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "__typename",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cursor",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PageInfo",
          "kind": "LinkedField",
          "name": "pageInfo",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "endCursor",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasNextPage",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "kind": "ClientExtension",
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "__id",
              "storageKey": null
            }
          ]
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "e9853a7cd93ddb62892018b433d931d2";

export default node;
