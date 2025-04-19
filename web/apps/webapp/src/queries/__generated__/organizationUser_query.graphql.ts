/**
 * @generated SignedSource<<aefe7ef5cddee0ce38688abe6c218e52>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
export type OrganizationMemberStatus = "Active" | "Inactive" | "%future added value";
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
        readonly organization: {
          readonly uniqueId: string;
        };
        readonly " $fragmentSpreads": FragmentRefs<"teamCard_TeamDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly myBillingContactDetails: {
    readonly addressLine1: string | null | undefined;
    readonly addressLine2: string | null | undefined;
    readonly city: string | null | undefined;
    readonly companyName: string | null | undefined;
    readonly country: string | null | undefined;
    readonly email: string | null | undefined;
    readonly id: string;
    readonly province: string | null | undefined;
    readonly suburb: string | null | undefined;
    readonly zipcode: string | null | undefined;
  };
  readonly organizationMembers: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly role: OrganizationMemberRole | null | undefined;
        readonly status: OrganizationMemberStatus;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationUserLeftSideNavigationMenuContent_query" | "teamCard_query">;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "email",
  "storageKey": null
},
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
  "fields": [
    {
      "kind": "Variable",
      "name": "customerId",
      "variableName": "customerId"
    },
    {
      "kind": "Variable",
      "name": "organizationId",
      "variableName": "organizationId"
    }
  ],
  "kind": "ObjectValue",
  "name": "where"
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
  "name": "uniqueId",
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
      "name": "organizationId"
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
      "selections": [
        (v0/*: any*/)
      ],
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
        (v1/*: any*/),
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
        }
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
        (v7/*: any*/)
      ],
      "concreteType": "TeamConnection",
      "kind": "LinkedField",
      "name": "__organizationUser_customerTeams_connection",
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
                (v0/*: any*/),
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Team_OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": [
                    (v9/*: any*/)
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
                        (v9/*: any*/),
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "TeamCustomerDetails",
                          "kind": "LinkedField",
                          "name": "customer",
                          "plural": false,
                          "selections": [
                            (v9/*: any*/),
                            (v4/*: any*/),
                            (v5/*: any*/),
                            (v6/*: any*/),
                            (v3/*: any*/),
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
        (v7/*: any*/)
      ],
      "concreteType": "OrganizationMemberConnection",
      "kind": "LinkedField",
      "name": "organizationMembers",
      "plural": false,
      "selections": [
        (v8/*: any*/),
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
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerBillingContactDetails",
      "kind": "LinkedField",
      "name": "myBillingContactDetails",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "companyName",
          "storageKey": null
        },
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "addressLine1",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "addressLine2",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "suburb",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "city",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "province",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "zipcode",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "country",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamCard_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationUserLeftSideNavigationMenuContent_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "e90b4b86b826ccdbff0af5ef526964d4";

export default node;
