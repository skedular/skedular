/**
 * @generated SignedSource<<0c6832648bf89dc6364128e2a746b5bb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationPeopleTab_query_organizationMembers$data = {
  readonly paginatedCustomersByDefaultLocation?: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly " $fragmentSpreads": FragmentRefs<"customerCard_CustomerDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "locationPeopleTab_query_organizationMembers";
};
export type locationPeopleTab_query_organizationMembers$key = {
  readonly " $data"?: locationPeopleTab_query_organizationMembers$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query_organizationMembers">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "paginatedCustomersByDefaultLocation"
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": 50,
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
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "locationOrganizationPeopleSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "peopleNameSearchText"
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
      "operation": require('./locationPeopleTab_query_paginatedCustomersByDefaultLocation.graphql')
    }
  },
  "name": "locationPeopleTab_query_organizationMembers",
  "selections": [
    {
      "condition": "locationExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": "paginatedCustomersByDefaultLocation",
          "args": [
            {
              "kind": "Variable",
              "name": "orderBy",
              "variableName": "locationOrganizationPeopleSortingValues"
            },
            {
              "fields": [
                {
                  "kind": "Variable",
                  "name": "locationId",
                  "variableName": "locationId"
                },
                {
                  "kind": "Variable",
                  "name": "nameContains",
                  "variableName": "peopleNameSearchText"
                }
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "CustomerConnection",
          "kind": "LinkedField",
          "name": "__locationPeopleTab_paginatedCustomersByDefaultLocation_connection",
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
              "concreteType": "CustomerEdge",
              "kind": "LinkedField",
              "name": "edges",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CustomerDetails",
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
                      "args": null,
                      "kind": "FragmentSpread",
                      "name": "customerCard_CustomerDetails"
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
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "99e1fde46e738ae374900e5c1c285dcf";

export default node;
