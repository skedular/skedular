/**
 * @generated SignedSource<<dbbab1963a1bc3243d47ce2862ae1d77>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type OrganizationMemberOrderField = "FamilyName" | "GivenName" | "MembershipType" | "MiddleName" | "Name" | "PhoneNumber" | "Status" | "%future added value";
export type TeamMemberOrderField = "FamilyName" | "GivenName" | "MembershipType" | "MiddleName" | "Name" | "%future added value";
export type TeamMemberOrderInput = {
  direction: OrderDirection;
  field: TeamMemberOrderField;
};
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type teamMembersTab_rootQuery$variables = {
  bookingPeopleNameSearchText?: string | null | undefined;
  organizationExists: boolean;
  organizationId: string;
  organizationMemberSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  teamId: string;
  teamMembersSortingValues?: ReadonlyArray<TeamMemberOrderInput> | null | undefined;
};
export type teamMembersTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"teamMembersTab_query" | "teamMembersTab_teamMembers_query">;
};
export type teamMembersTab_rootQuery = {
  response: teamMembersTab_rootQuery$data;
  variables: teamMembersTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingPeopleNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMemberSelectorOrganizationMembersSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamMembersSortingValues"
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v10 = [
  (v9/*: any*/)
],
v11 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 20
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMemberSelectorOrganizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "bookingPeopleNameSearchText"
      },
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
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v18 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v19 = {
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
v20 = {
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
v21 = [
  "where",
  "orderBy"
],
v22 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "teamMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      },
      {
        "kind": "Variable",
        "name": "teamId",
        "variableName": "teamId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v23 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": [
    (v8/*: any*/),
    (v13/*: any*/),
    (v14/*: any*/),
    (v15/*: any*/),
    (v16/*: any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/),
      (v6/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "teamMembersTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamMembersTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamMembersTab_teamMembers_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v5/*: any*/),
      (v0/*: any*/),
      (v6/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Operation",
    "name": "teamMembersTab_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "teamId"
          }
        ],
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          },
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
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v8/*: any*/)
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
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
            "selections": [
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamCustomerDetails",
                "kind": "LinkedField",
                "name": "customer",
                "plural": false,
                "selections": (v10/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamOrganizationMemberDetails",
                "kind": "LinkedField",
                "name": "organizationMember",
                "plural": false,
                "selections": (v10/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
            "args": (v11/*: any*/),
            "concreteType": "OrganizationMemberConnection",
            "kind": "LinkedField",
            "name": "organizationMembers",
            "plural": false,
            "selections": [
              (v12/*: any*/),
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
                      (v7/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationCustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v9/*: any*/),
                          (v8/*: any*/),
                          (v13/*: any*/),
                          (v14/*: any*/),
                          (v15/*: any*/),
                          (v16/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v17/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v18/*: any*/)
                ],
                "storageKey": null
              },
              (v19/*: any*/),
              (v20/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
            "args": (v11/*: any*/),
            "filters": (v21/*: any*/),
            "handle": "connection",
            "key": "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers",
            "kind": "LinkedHandle",
            "name": "organizationMembers"
          }
        ]
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "concreteType": "TeamMemberConnection",
        "kind": "LinkedField",
        "name": "teamMembers",
        "plural": false,
        "selections": [
          (v12/*: any*/),
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
                  (v7/*: any*/),
                  (v23/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "organizationMember",
                    "plural": false,
                    "selections": [
                      (v23/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v17/*: any*/)
                ],
                "storageKey": null
              },
              (v18/*: any*/)
            ],
            "storageKey": null
          },
          (v19/*: any*/),
          (v20/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v22/*: any*/),
        "filters": (v21/*: any*/),
        "handle": "connection",
        "key": "teamMembersTab_teamMembers",
        "kind": "LinkedHandle",
        "name": "teamMembers"
      }
    ]
  },
  "params": {
    "cacheID": "3a86215814878d32f60c925afea5d886",
    "id": null,
    "metadata": {},
    "name": "teamMembersTab_rootQuery",
    "operationKind": "query",
    "text": "query teamMembersTab_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $teamId: String!\n  $bookingPeopleNameSearchText: String\n  $teamMembersSortingValues: [TeamMemberOrderInput!]\n  $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $peopleNameSearchText: String\n) {\n  ...teamMembersTab_query\n  ...teamMembersTab_teamMembers_query\n}\n\nfragment organizationMemberSelector_query on Query {\n  organizationMemberSelectorPaginatedOrganizationMembers: organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment teamMemberCard_TeamMemberDetails on TeamMemberDetails {\n  id\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationMember {\n    customer {\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n}\n\nfragment teamMemberCard_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    canModify\n    members {\n      id\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n    }\n  }\n}\n\nfragment teamMembersTab_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    timezone\n    organization {\n      name\n    }\n    canModify\n    members {\n      id\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n    }\n  }\n  ...teamMemberCard_query\n  ...organizationMemberSelector_query\n}\n\nfragment teamMembersTab_teamMembers_query on Query {\n  teamMembers(first: 50, where: {teamId: $teamId, nameContains: $peopleNameSearchText}, orderBy: $teamMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...teamMemberCard_TeamMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "c5ace730d4151103795e3cf555d6ec72";

export default node;
