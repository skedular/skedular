/**
 * @generated SignedSource<<48d7649c2be70d095935509ee638b72c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "About" | "Name" | "Timezone" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type TeamOrderField = "About" | "Name" | "%future added value";
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type teams_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  primaryLocationIds?: ReadonlyArray<string> | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type teams_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationSelector_allLocations_query" | "myTeams_teams_query">;
};
export type teams_rootQuery = {
  response: teams_rootQuery$data;
  variables: teams_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "primaryLocationIds"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamsSortingValues"
},
v4 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v8 = {
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
},
v9 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "teamsSortingValues"
  },
  {
    "fields": [
      (v4/*: any*/),
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
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "teams_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationSelector_allLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myTeams_teams_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "teams_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              (v4/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v6/*: any*/),
                  (v7/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v8/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "concreteType": "TeamConnection",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v5/*: any*/),
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
                  (v6/*: any*/),
                  (v7/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      (v10/*: any*/)
                    ],
                    "storageKey": null
                  },
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
                          (v10/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "TeamCustomerDetails",
                            "kind": "LinkedField",
                            "name": "customer",
                            "plural": false,
                            "selections": [
                              (v10/*: any*/),
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
                              (v7/*: any*/),
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
                      (v6/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasFutureBooking",
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
          (v8/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v9/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "myTeams_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      }
    ]
  },
  "params": {
    "cacheID": "d633716b513cb6d58268b298f92218ba",
    "id": null,
    "metadata": {},
    "name": "teams_rootQuery",
    "operationKind": "query",
    "text": "query teams_rootQuery(\n  $organizationId: String!\n  $primaryLocationIds: [String!]\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  ...locationSelector_allLocations_query\n  ...myTeams_teams_query\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment myTeamCard_TeamDetails on TeamDetails {\n  id\n  name\n  organization {\n    uniqueId\n  }\n  members {\n    organizationMember {\n      uniqueId\n      customer {\n        uniqueId\n        givenName\n        middleName\n        familyName\n        name\n        photoUrl\n      }\n    }\n    id\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n}\n\nfragment myTeams_teams_query on Query {\n  teams(where: {organizationId: $organizationId, primaryLocationIds: $primaryLocationIds}, orderBy: $teamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueId\n        }\n        members {\n          organizationMember {\n            uniqueId\n            customer {\n              uniqueId\n              givenName\n              middleName\n              familyName\n              name\n              photoUrl\n            }\n          }\n          id\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        ...myTeamCard_TeamDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "134764509a00a86dd7e2d87f1281b20a";

export default node;
