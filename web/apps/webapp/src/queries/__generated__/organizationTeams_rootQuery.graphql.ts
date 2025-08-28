/**
 * @generated SignedSource<<de1a9586bd8d4a0964d5c01fbdd71625>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type TeamOrderField = "ABOUT" | "NAME" | "%future added value";
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type organizationTeams_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
  primaryLocationIds?: ReadonlyArray<string> | null | undefined;
  teamsSortingValues?: ReadonlyArray<TeamOrderInput> | null | undefined;
};
export type organizationTeams_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly preferredTeams: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  };
  readonly " $fragmentSpreads": FragmentRefs<"locationSelector_allLocations_query" | "organizationTeams_teams_query" | "teamCard_query">;
};
export type organizationTeams_rootQuery = {
  response: organizationTeams_rootQuery$data;
  variables: organizationTeams_rootQuery$variables;
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
  "name": "organizationUniqueAlphanumericName"
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
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v4/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerTeamDetails",
      "kind": "LinkedField",
      "name": "preferredTeams",
      "plural": true,
      "selections": [
        (v5/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v7 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = {
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
v11 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "teamsSortingValues"
  },
  {
    "fields": [
      (v7/*: any*/),
      {
        "kind": "Variable",
        "name": "primaryLocationIds",
        "variableName": "primaryLocationIds"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
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
    "name": "organizationTeams_rootQuery",
    "selections": [
      (v6/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamCard_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationSelector_allLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTeams_teams_query"
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
    "name": "organizationTeams_rootQuery",
    "selections": [
      (v6/*: any*/),
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
              (v7/*: any*/)
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v8/*: any*/),
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
                  (v4/*: any*/),
                  (v9/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v8/*: any*/),
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
                  (v4/*: any*/),
                  (v9/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Team_OrganizationDetails",
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
                          (v5/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "concreteType": "Team_CustomerDetails",
                            "kind": "LinkedField",
                            "name": "customer",
                            "plural": false,
                            "selections": [
                              (v5/*: any*/),
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
                              (v9/*: any*/),
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
                      (v4/*: any*/)
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
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "primaryFeatureImage",
                    "plural": false,
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
          (v10/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v11/*: any*/),
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
    "cacheID": "ca16fa47b7f499c0163b5ce1198d27f9",
    "id": null,
    "metadata": {},
    "name": "organizationTeams_rootQuery",
    "operationKind": "query",
    "text": "query organizationTeams_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $primaryLocationIds: [String!]\n  $teamsSortingValues: [TeamOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  me {\n    id\n    preferredTeams {\n      uniqueId\n    }\n  }\n  ...teamCard_query\n  ...locationSelector_allLocations_query\n  ...organizationTeams_teams_query\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment organizationTeams_teams_query on Query {\n  teams(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, primaryLocationIds: $primaryLocationIds}, orderBy: $teamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueAlphanumericName\n        }\n        members {\n          organizationMember {\n            uniqueId\n            customer {\n              uniqueId\n              givenName\n              middleName\n              familyName\n              name\n              photoUrl\n            }\n          }\n          id\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        ...teamCard_TeamDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment teamCard_TeamDetails on TeamDetails {\n  id\n  name\n  organization {\n    uniqueAlphanumericName\n  }\n  members {\n    organizationMember {\n      uniqueId\n      customer {\n        uniqueId\n        givenName\n        middleName\n        familyName\n        name\n        photoUrl\n      }\n    }\n    id\n  }\n  primaryFeatureImage {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n}\n\nfragment teamCard_query on Query {\n  me {\n    id\n    preferredTeams {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7998f2303c0785dc8f34af1c56239c93";

export default node;
