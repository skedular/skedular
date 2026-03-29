/**
 * @generated SignedSource<<b691d3122354893208b76d027feb14ae>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type TeamOrderField = "ABOUT" | "NAME" | "%future added value";
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type organizationTeams_teams_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  primaryLocationIds?: ReadonlyArray<string> | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type organizationTeams_teams_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeams_teams_query">;
};
export type organizationTeams_teams_refetchableFragment = {
  response: organizationTeams_teams_refetchableFragment$data;
  variables: organizationTeams_teams_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "primaryLocationIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamsSortingValues"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationTeams_teams_refetchableFragment",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "organizationTeams_teams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeams_teams_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
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
                  (v2/*: any*/),
                  (v3/*: any*/),
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
                      },
                      (v2/*: any*/)
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
                                      (v3/*: any*/),
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
                              },
                              (v2/*: any*/)
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
                    "alias": null,
                    "args": null,
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "featureImages",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "thumbnail",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "url",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "height",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "width",
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
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationTeams_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      }
    ]
  },
  "params": {
    "cacheID": "08c9eb01c7dfab370ab21ec806421625",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_teams_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationTeams_teams_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $organizationCustomDomain: String\n  $primaryLocationIds: [String!]\n  $teamsSortingValues: [TeamOrderInput!]\n) {\n  ...organizationTeams_teams_query_1G22uz\n}\n\nfragment organizationTeams_teams_query_1G22uz on Query {\n  teams(first: $count, after: $cursor, where: {organizationCustomDomain: $organizationCustomDomain, primaryLocationIds: $primaryLocationIds}, orderBy: $teamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          customDomain\n          id\n        }\n        members {\n          edges {\n            node {\n              organizationMember {\n                uniqueId\n                customer {\n                  id\n                  givenName\n                  middleName\n                  familyName\n                  name\n                  photoUrl\n                }\n              }\n              id\n            }\n          }\n        }\n        canModify\n        canDelete\n        ...teamCard_TeamDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment teamCard_TeamDetails on TeamDetails {\n  id\n  name\n  organization {\n    customDomain\n    id\n  }\n  members {\n    edges {\n      node {\n        organizationMember {\n          uniqueId\n          customer {\n            id\n            givenName\n            middleName\n            familyName\n            name\n            photoUrl\n          }\n        }\n        id\n      }\n    }\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  canModify\n  canDelete\n}\n"
  }
};
})();

(node as any).hash = "529692b1b50c7c8ecac2cb1ee48293e5";

export default node;
