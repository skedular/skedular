/**
 * @generated SignedSource<<aa25336e75d358b53f4de640ca1ae911>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMarketplaceSetup_locationTags_query$data = {
  readonly locationTags: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly color: string | null | undefined;
        readonly description: string | null | undefined;
        readonly id: string;
        readonly name: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "organizationMarketplaceSetup_locationTags_query";
};
export type organizationMarketplaceSetup_locationTags_query$key = {
  readonly " $data"?: organizationMarketplaceSetup_locationTags_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMarketplaceSetup_locationTags_query">;
};

import organizationMarketplaceSetup_locationTags_refetchableFragment_graphql from './organizationMarketplaceSetup_locationTags_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "locationTags"
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
      "name": "locationTagNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
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
      "operation": organizationMarketplaceSetup_locationTags_refetchableFragment_graphql
    }
  },
  "name": "organizationMarketplaceSetup_locationTags_query",
  "selections": [
    {
      "alias": "locationTags",
      "args": [
        {
          "kind": "Literal",
          "name": "orderBy",
          "value": [
            {
              "direction": "Ascending",
              "field": "Name"
            }
          ]
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "nameContains",
              "variableName": "locationTagNameSearchText"
            },
            {
              "kind": "Variable",
              "name": "organizationId",
              "variableName": "organizationId"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "OrganizationTagConnection",
      "kind": "LinkedField",
      "name": "__organizationMarketplaceSetup_locationTags_connection",
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
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "afc58f1f3ece4ce2f642a5eef8ec5408";

export default node;
