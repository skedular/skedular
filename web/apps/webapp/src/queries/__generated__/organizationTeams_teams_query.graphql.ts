/**
 * @generated SignedSource<<fa07a1b0ed4a5b9045f5ff1f5843cb02>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTeams_teams_query$data = {
  readonly teams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly canDelete: boolean;
        readonly canModify: boolean;
        readonly id: string;
        readonly members: {
          readonly edges: ReadonlyArray<{
            readonly node: {
              readonly organizationMember: {
                readonly customer: {
                  readonly familyName: string | null | undefined;
                  readonly givenName: string | null | undefined;
                  readonly id: string;
                  readonly middleName: string | null | undefined;
                  readonly name: string | null | undefined;
                  readonly photoUrl: string | null | undefined;
                };
                readonly uniqueId: string;
              } | null | undefined;
            };
          }>;
        };
        readonly name: string;
        readonly organization: {
          readonly customDomain: string | null | undefined;
        };
        readonly " $fragmentSpreads": FragmentRefs<"teamCard_TeamDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "organizationTeams_teams_query";
};
export type organizationTeams_teams_query$key = {
  readonly " $data"?: organizationTeams_teams_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeams_teams_query">;
};

import organizationTeams_teams_refetchableFragment_graphql from './organizationTeams_teams_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "teams"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
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
      "name": "organizationCustomDomain"
    },
    {
      "kind": "RootArgument",
      "name": "primaryLocationIds"
    },
    {
      "kind": "RootArgument",
      "name": "teamsSortingValues"
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
      "operation": organizationTeams_teams_refetchableFragment_graphql
    }
  },
  "name": "organizationTeams_teams_query",
  "selections": [
    {
      "alias": "teams",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "teamsSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "organizationCustomDomain",
              "variableName": "organizationCustomDomain"
            },
            {
              "kind": "Variable",
              "name": "primaryLocationIds",
              "variableName": "primaryLocationIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfTeamEdge",
      "kind": "LinkedField",
      "name": "__organizationTeams_teams_connection",
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
                (v1/*: any*/),
                (v2/*: any*/),
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
                      "name": "customDomain",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ConnectionOfTeamMemberEdge",
                  "kind": "LinkedField",
                  "name": "members",
                  "plural": false,
                  "selections": [
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
                              "concreteType": "TeamOrganizationMemberDetails",
                              "kind": "LinkedField",
                              "name": "organizationMember",
                              "plural": false,
                              "selections": [
                                {
                                  "alias": null,
                                  "args": null,
                                  "kind": "ScalarField",
                                  "name": "uniqueId",
                                  "storageKey": null
                                },
                                {
                                  "alias": null,
                                  "args": null,
                                  "concreteType": "CustomerDetails",
                                  "kind": "LinkedField",
                                  "name": "customer",
                                  "plural": false,
                                  "selections": [
                                    (v1/*: any*/),
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
                                    (v2/*: any*/),
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
                        }
                      ],
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "canModify",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "canDelete",
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "teamCard_TeamDetails"
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

(node as any).hash = "529692b1b50c7c8ecac2cb1ee48293e5";

export default node;
