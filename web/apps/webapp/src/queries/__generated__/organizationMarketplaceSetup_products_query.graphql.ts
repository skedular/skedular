/**
 * @generated SignedSource<<19267bae44ed33b10a9814c78e936010>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_products_query$data = {
  readonly products: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookAllLocationResources: boolean;
        readonly description: string | null | undefined;
        readonly id: string;
        readonly inactive: boolean;
        readonly isPriceTaxInclusive: boolean;
        readonly maxBookingSpreadDays: number | null | undefined;
        readonly maxDurationMinutes: number | null | undefined;
        readonly minDurationMinutes: number | null | undefined;
        readonly name: string;
        readonly numberOfResourcesToBook: number;
        readonly organization: {
          readonly uniqueAlphanumericName: string | null | undefined;
        };
        readonly priceToDisplay: string;
        readonly priceUnit: {
          readonly name: string;
        };
        readonly recurrenceWindowDays: number;
        readonly requireConsecutiveDays: boolean;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "organizationMarketplaceSetup_products_query";
};
export type organizationMarketplaceSetup_products_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_products_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_products_query">;
};

import organizationMarketplaceSetup_products_refetchableFragment_graphql from './organizationMarketplaceSetup_products_refetchableFragment.graphql';

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
};
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
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "productNameSearchText"
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
      "operation": organizationMarketplaceSetup_products_refetchableFragment_graphql
    }
  },
  "name": "organizationMarketplaceSetup_products_query",
  "selections": [
    {
      "alias": "products",
      "args": [
        {
          "kind": "Literal",
          "name": "orderBy",
          "value": [
            {
              "direction": "ASCENDING",
              "field": "NAME"
            }
          ]
        },
        {
          "fields": [
            {
              "kind": "Literal",
              "name": "includeInactive",
              "value": true
            },
            {
              "kind": "Variable",
              "name": "nameContains",
              "variableName": "productNameSearchText"
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
      "name": "__organizationMarketplaceSetup_products_connection",
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
                  "name": "priceToDisplay",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "PriceUnitDetails",
                  "kind": "LinkedField",
                  "name": "priceUnit",
                  "plural": false,
                  "selections": [
                    (v1/*: any*/)
                  ],
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
                  "concreteType": "OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "uniqueAlphanumericName",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "isPriceTaxInclusive",
                  "storageKey": null
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

(node as any).hash = "9b7d80d9533957df64b4a4eb1c3f02bc";

export default node;
