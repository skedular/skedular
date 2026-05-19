/**
 * @generated SignedSource<<363b6fde1859c1762ab82ab20f6fd5fe>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
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
        readonly featureImages: ReadonlyArray<{
          readonly thumbnail: {
            readonly url: string;
          } | null | undefined;
        }>;
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
          readonly status: {
            readonly name: string;
            readonly type: OrganizationMemberStatus;
          };
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
  (v0/*:: as any*/)
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
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*:: as any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "PersonalInformationVisibilityDetails",
  "kind": "LinkedField",
  "name": "personalInformationVisibility",
  "plural": false,
  "selections": (v7/*:: as any*/),
  "storageKey": null
},
v9 = {
  "kind": "Variable",
  "name": "customerId",
  "variableName": "customerId"
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v11 = {
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
      "selections": (v1/*:: as any*/),
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
        (v0/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "email",
          "storageKey": null
        },
        (v2/*:: as any*/),
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
        (v3/*:: as any*/),
        (v4/*:: as any*/),
        (v5/*:: as any*/),
        (v6/*:: as any*/),
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
        (v8/*:: as any*/)
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
            (v9/*:: as any*/),
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
        (v10/*:: as any*/),
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
                (v0/*:: as any*/),
                (v3/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": (v1/*:: as any*/),
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
                                    (v0/*:: as any*/),
                                    (v4/*:: as any*/),
                                    (v5/*:: as any*/),
                                    (v6/*:: as any*/),
                                    (v3/*:: as any*/),
                                    (v2/*:: as any*/),
                                    (v8/*:: as any*/)
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
        (v11/*:: as any*/)
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
                (v9/*:: as any*/)
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
            (v10/*:: as any*/),
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
                    (v0/*:: as any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationMemberStatusDetails",
                      "kind": "LinkedField",
                      "name": "status",
                      "plural": false,
                      "selections": (v7/*:: as any*/),
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            (v11/*:: as any*/)
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

(node as any).hash = "9db458c2b5523ecf22501a2033493b03";

export default node;
