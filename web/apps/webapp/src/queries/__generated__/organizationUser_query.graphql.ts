/**
 * @generated SignedSource<<3f5cbde0ef8bcfdcdb54c689b51491e8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type OrganizationMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type PersonalInformationVisibility = "REDACTED" | "VISIBLE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationUser_query$data = {
  readonly customer: {
    readonly designation: string | null | undefined;
    readonly email: string | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly personalInformationVisibility: {
      readonly name: string;
      readonly type: PersonalInformationVisibility;
    };
    readonly phoneNumber: string | null | undefined;
    readonly photoUrl: string | null | undefined;
    readonly timezone: string | null | undefined;
    readonly title: string | null | undefined;
  } | null | undefined;
  readonly customerTeams: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
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
                  readonly personalInformationVisibility: {
                    readonly name: string;
                    readonly type: PersonalInformationVisibility;
                  };
                  readonly photoUrl: string | null | undefined;
                };
                readonly uniqueId: string;
              } | null | undefined;
            };
          }>;
        };
        readonly name: string;
        readonly organization: {
          readonly id: string;
        };
        readonly " $fragmentSpreads": FragmentRefs<"teamCard_TeamDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly me: {
    readonly id: string;
  };
  readonly organization: {
    readonly members: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly id: string;
          readonly role: OrganizationMemberRole | null | undefined;
          readonly status: OrganizationMemberStatus;
        };
      }>;
      readonly totalCount: number;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUserLeftSideNavigationMenuContent_query" | "singleChoiceUserPersonalInformationVisibility_query">;
  readonly " $fragmentType": "organizationUser_query";
};
export type organizationUser_query$key = {
  readonly " $data"?: organizationUser_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUser_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  (v0/*: any*/)
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
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
  "name": "givenName",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "PersonalInformationVisibilityDetails",
  "kind": "LinkedField",
  "name": "personalInformationVisibility",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "type",
      "storageKey": null
    },
    (v3/*: any*/)
  ],
  "storageKey": null
},
v8 = {
  "kind": "Variable",
  "name": "customerId",
  "variableName": "customerId"
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
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
      "name": "customerId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
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
        "path": [
          "customerTeams"
        ]
      }
    ]
  },
  "name": "organizationUser_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": (v1/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "customerId"
        }
      ],
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "email",
          "storageKey": null
        },
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "designation",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "title",
          "storageKey": null
        },
        (v3/*: any*/),
        (v4/*: any*/),
        (v5/*: any*/),
        (v6/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "timezone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "phoneNumber",
          "storageKey": null
        },
        (v7/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": "customerTeams",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "teamsSortingValues"
        },
        {
          "fields": [
            (v8/*: any*/),
            {
              "kind": "Variable",
              "name": "organizationCustomDomain",
              "variableName": "organizationCustomDomain"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfTeamEdge",
      "kind": "LinkedField",
      "name": "__organizationUser_customerTeams_connection",
      "plural": false,
      "selections": [
        (v9/*: any*/),
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
                (v0/*: any*/),
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": (v1/*: any*/),
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
                                    (v0/*: any*/),
                                    (v4/*: any*/),
                                    (v5/*: any*/),
                                    (v6/*: any*/),
                                    (v3/*: any*/),
                                    (v2/*: any*/),
                                    (v7/*: any*/)
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
      "args": [
        {
          "kind": "Variable",
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "fields": [
                (v8/*: any*/)
              ],
              "kind": "ObjectValue",
              "name": "where"
            }
          ],
          "concreteType": "ConnectionOfOrganizationMemberEdge",
          "kind": "LinkedField",
          "name": "members",
          "plural": false,
          "selections": [
            (v9/*: any*/),
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
                    (v0/*: any*/),
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
                    }
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            (v10/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationUserLeftSideNavigationMenuContent_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceUserPersonalInformationVisibility_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "fe93c8330a115c0a3fbc8ed38fec3d33";

export default node;
