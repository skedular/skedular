/**
 * @generated SignedSource<<76de2d99d1e19d36dc93404c9b0d2ce9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMembers_teamsrefetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  organizationId?: string | null | undefined;
};
export type organizationMembers_teamsrefetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationMembers_teams_query">;
};
export type organizationMembers_teamsrefetchableFragment = {
  response: organizationMembers_teamsrefetchableFragment$data;
  variables: organizationMembers_teamsrefetchableFragment$variables;
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
    "name": "organizationId"
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
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMembers_teamsrefetchableFragment",
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
        "name": "organizationMembers_teams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembers_teamsrefetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamConnection",
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
                          (v4/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "TeamCustomerDetails",
                            "kind": "LinkedField",
                            "name": "customer",
                            "plural": false,
                            "selections": [
                              (v4/*: any*/),
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
          "where"
        ],
        "handle": "connection",
        "key": "myTeams_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      }
    ]
  },
  "params": {
    "cacheID": "55da1ba6e9801b3c770e8159f87b5315",
    "id": null,
    "metadata": {},
    "name": "organizationMembers_teamsrefetchableFragment",
    "operationKind": "query",
    "text": "query organizationMembers_teamsrefetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $organizationId: String\n) {\n  ...organizationMembers_teams_query_1G22uz\n}\n\nfragment myTeamCard_TeamDetails on TeamDetails {\n  id\n  name\n  members {\n    organizationMember {\n      uniqueId\n      customer {\n        uniqueId\n        givenName\n        middleName\n        familyName\n        name\n        photoUrl\n      }\n    }\n    id\n  }\n}\n\nfragment organizationMembers_teams_query_1G22uz on Query {\n  teams(first: $count, after: $cursor, where: {organizationId: $organizationId}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        members {\n          organizationMember {\n            uniqueId\n            customer {\n              uniqueId\n              givenName\n              middleName\n              familyName\n              name\n              photoUrl\n            }\n          }\n          id\n        }\n        ...myTeamCard_TeamDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "150db3da2a1bc44a320b6870e7c0b212";

export default node;
