/**
 * @generated SignedSource<<ce466767d1b27394bc187b75dc5d7288>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_productTags_query$data = {
  readonly organization: {
    readonly productTags: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly color: string | null | undefined;
          readonly description: string | null | undefined;
          readonly id: string;
          readonly name: string;
        };
      }>;
      readonly totalCount: number;
    };
  } | null | undefined;
  readonly " $fragmentType": "organizationMarketplaceSetup_productTags_query";
};
export type organizationMarketplaceSetup_productTags_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_productTags_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_productTags_query">;
};

import organizationMarketplaceSetup_productTags_refetchableFragment_graphql from './organizationMarketplaceSetup_productTags_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "organization",
  "productTags"
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
      "name": "organizationCustomDomain"
    },
    {
      "kind": "RootArgument",
      "name": "productTagNameSearchText"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*:: as any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*:: as any*/)
      },
      "fragmentPathInResult": [],
      "operation": organizationMarketplaceSetup_productTags_refetchableFragment_graphql
    }
  },
  "name": "organizationMarketplaceSetup_productTags_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": "productTags",
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
                  "kind": "Variable",
                  "name": "nameContains",
                  "variableName": "productTagNameSearchText"
                }
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "ConnectionOfOrganizationTagEdge",
          "kind": "LinkedField",
          "name": "__organizationMarketplaceSetup_productTags_connection",
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
              "concreteType": "OrganizationTagEdge",
              "kind": "LinkedField",
              "name": "edges",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationTagDetails",
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
                      "name": "name",
                      "storageKey": null
                    },
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
                      "name": "color",
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
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "8af29494aabd5b84731fad13c4fbfe36";

export default node;
