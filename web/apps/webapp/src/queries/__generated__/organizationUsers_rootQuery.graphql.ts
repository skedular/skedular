/**
 * @generated SignedSource<<f555b07653618ce0d88552dbad42561d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type organizationUsers_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
  peopleNameSearchText?: string | null | undefined;
};
export type organizationUsers_rootQuery$data = {
  readonly organization: {
    readonly canInvitePeople: boolean;
  } | null | undefined;
  readonly organizationMemberRoles: ReadonlyArray<OrganizationMemberRole>;
  readonly teams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly members: {
          readonly edges: ReadonlyArray<{
            readonly node: {
              readonly organizationMember: {
                readonly customer: {
                  readonly id: string;
                };
                readonly uniqueId: string;
              } | null | undefined;
            };
          }>;
        };
        readonly name: string;
        readonly " $fragmentSpreads": FragmentRefs<"teamCard_TeamDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationUsers_organizationMembers_query" | "teamSelector_allTeams_query">;
};
export type organizationUsers_rootQuery = {
  response: organizationUsers_rootQuery$data;
  variables: organizationUsers_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "peopleNameSearchText"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canInvitePeople",
  "storageKey": null
},
v3 = [
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "organizationUniqueAlphanumericName",
        "variableName": "organizationUniqueAlphanumericName"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "organizationMemberRoles",
  "storageKey": null
},
v10 = [
  {
    "fields": [
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
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationUsers_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v4/*: any*/),
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
                  (v5/*: any*/),
                  (v6/*: any*/),
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
                                  (v7/*: any*/),
                                  {
                                    "alias": null,
                                    "args": null,
                                    "concreteType": "CustomerDetails",
                                    "kind": "LinkedField",
                                    "name": "customer",
                                    "plural": false,
                                    "selections": [
                                      (v5/*: any*/)
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
                    "args": null,
                    "kind": "FragmentSpread",
                    "name": "teamCard_TeamDetails"
                  }
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
      (v9/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamSelector_allTeams_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationUsers_organizationMembers_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUsers_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v5/*: any*/),
          {
            "alias": null,
            "args": (v10/*: any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v5/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v5/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "email",
                            "storageKey": null
                          },
                          (v6/*: any*/),
                          (v11/*: any*/),
                          (v12/*: any*/),
                          (v13/*: any*/),
                          (v14/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "phoneNumber",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "status",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "role",
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
            "args": (v10/*: any*/),
            "filters": [
              "where"
            ],
            "handle": "connection",
            "key": "organizationMembers_members",
            "kind": "LinkedHandle",
            "name": "members"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "ConnectionOfTeamEdge",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v4/*: any*/),
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
                  (v5/*: any*/),
                  (v6/*: any*/),
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
                                  (v7/*: any*/),
                                  {
                                    "alias": null,
                                    "args": null,
                                    "concreteType": "CustomerDetails",
                                    "kind": "LinkedField",
                                    "name": "customer",
                                    "plural": false,
                                    "selections": [
                                      (v5/*: any*/),
                                      (v11/*: any*/),
                                      (v12/*: any*/),
                                      (v13/*: any*/),
                                      (v6/*: any*/),
                                      (v14/*: any*/)
                                    ],
                                    "storageKey": null
                                  }
                                ],
                                "storageKey": null
                              },
                              (v5/*: any*/)
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
                    "concreteType": "OrganizationDetails",
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
                      },
                      (v5/*: any*/)
                    ],
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
                  }
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
      (v9/*: any*/)
    ]
  },
  "params": {
    "cacheID": "4d7cfa1cfee4ef13f6d707935e2ec0a2",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_rootQuery",
    "operationKind": "query",
    "text": "query organizationUsers_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $peopleNameSearchText: String\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    canInvitePeople\n    id\n  }\n  teams(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        members {\n          edges {\n            node {\n              organizationMember {\n                uniqueId\n                customer {\n                  id\n                }\n              }\n              id\n            }\n          }\n        }\n        ...teamCard_TeamDetails\n      }\n    }\n  }\n  organizationMemberRoles\n  ...teamSelector_allTeams_query\n  ...organizationUsers_organizationMembers_query\n}\n\nfragment organizationUsers_organizationMembers_query on Query {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    members(where: {nameContains: $peopleNameSearchText}) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            email\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n            phoneNumber\n          }\n          status\n          role\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment teamCard_TeamDetails on TeamDetails {\n  id\n  name\n  organization {\n    uniqueAlphanumericName\n    id\n  }\n  members {\n    edges {\n      node {\n        organizationMember {\n          uniqueId\n          customer {\n            id\n            givenName\n            middleName\n            familyName\n            name\n            photoUrl\n          }\n        }\n        id\n      }\n    }\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n}\n\nfragment teamSelector_allTeams_query on Query {\n  teams(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "86ca577a99efd85d4f1dc9ac1d71eabd";

export default node;
