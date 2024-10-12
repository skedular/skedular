/**
 * @generated SignedSource<<aea78fd25c9b0179e82666a31610571a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamPeopleTab_paginatedTeamMembers_query$data = {
  readonly paginatedTeamMembers?: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly " $fragmentSpreads": FragmentRefs<"teamMemberCard_TeamMemberDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "teamPeopleTab_paginatedTeamMembers_query";
};
export type teamPeopleTab_paginatedTeamMembers_query$key = {
  readonly " $data"?: teamPeopleTab_paginatedTeamMembers_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamPeopleTab_paginatedTeamMembers_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "paginatedTeamMembers"
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
      "name": "peopleNameSearchText"
    },
    {
      "kind": "RootArgument",
      "name": "teamExists"
    },
    {
      "kind": "RootArgument",
      "name": "teamId"
    },
    {
      "kind": "RootArgument",
      "name": "teamPeopleSortingValues"
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
      "operation": require('./teamPeopleTab_paginatedTeamMembers_refetchableFragment.graphql')
    }
  },
  "name": "teamPeopleTab_paginatedTeamMembers_query",
  "selections": [
    {
      "condition": "teamExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": "paginatedTeamMembers",
          "args": [
            {
              "kind": "Variable",
              "name": "orderBy",
              "variableName": "teamPeopleSortingValues"
            },
            {
              "fields": [
                {
                  "kind": "Variable",
                  "name": "nameContains",
                  "variableName": "peopleNameSearchText"
                },
                {
                  "kind": "Variable",
                  "name": "teamId",
                  "variableName": "teamId"
                }
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "TeamMemberConnection",
          "kind": "LinkedField",
          "name": "__teamPeopleTab_paginatedTeamMembers_connection",
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
              "concreteType": "TeamMemberEdge",
              "kind": "LinkedField",
              "name": "edges",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "TeamMemberDetails",
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
                      "name": "teamMemberCard_TeamMemberDetails"
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

(node as any).hash = "8798b703fc75f23e259d328ebd493e2a";

export default node;
