/**
 * @generated SignedSource<<49e9423228dad105d82a22051003f9ee>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMembers_teams_query$data = {
  readonly teams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly members: ReadonlyArray<{
          readonly organizationMember: {
            readonly customer: {
              readonly familyName: string | null | undefined;
              readonly givenName: string | null | undefined;
              readonly middleName: string | null | undefined;
              readonly name: string | null | undefined;
              readonly photoUrl: string | null | undefined;
              readonly uniqueId: string;
            };
            readonly uniqueId: string;
          } | null | undefined;
        }>;
        readonly name: string;
        readonly " $fragmentSpreads": FragmentRefs<"myTeamCard_TeamDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "organizationMembers_teams_query";
};
export type organizationMembers_teams_query$key = {
  readonly " $data"?: organizationMembers_teams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMembers_teams_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "teams"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
      "operation": require('./organizationMembers_teamsrefetchableFragment.graphql')
    }
  },
  "name": "organizationMembers_teams_query",
  "selections": [
    {
      "alias": "teams",
      "args": [
        {
          "fields": [
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
      "concreteType": "TeamConnection",
      "kind": "LinkedField",
      "name": "__myTeams_teams_connection",
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
          "concreteType": "TeamEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "TeamDetails",
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
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "TeamMemberDetails",
                  "kind": "LinkedField",
                  "name": "members",
                  "plural": true,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "TeamOrganizationMemberDetails",
                      "kind": "LinkedField",
                      "name": "organizationMember",
                      "plural": false,
                      "selections": [
                        (v2/*: any*/),
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "TeamCustomerDetails",
                          "kind": "LinkedField",
                          "name": "customer",
                          "plural": false,
                          "selections": [
                            (v2/*: any*/),
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "givenName",
                              "storageKey": null
                            },
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "middleName",
                              "storageKey": null
                            },
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "familyName",
                              "storageKey": null
                            },
                            (v1/*: any*/),
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "photoUrl",
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
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "myTeamCard_TeamDetails"
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

(node as any).hash = "150db3da2a1bc44a320b6870e7c0b212";

export default node;
