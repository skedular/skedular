/**
 * @generated SignedSource<<21f0278f55ca627874e9f3b766814136>>
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
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type teamMembersTab_refetchableFragment$variables = {
  bookingPeopleNameSearchText?: string | null | undefined;
  organizationId: string;
  organizationMemberSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  teamId: string;
};
export type teamMembersTab_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"teamMembersTab_query">;
};
export type teamMembersTab_refetchableFragment = {
  response: teamMembersTab_refetchableFragment$data;
  variables: teamMembersTab_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingPeopleNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationMemberSelectorOrganizationMembersSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamId"
  }
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
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v4 = [
  (v3/*: any*/)
],
v5 = [
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamMembersTab_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "teamMembersTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamMembersTab_refetchableFragment",
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
          (v1/*: any*/),
          (v2/*: any*/),
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
              (v2/*: any*/)
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
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamCustomerDetails",
                "kind": "LinkedField",
                "name": "customer",
                "plural": false,
                "selections": (v4/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamOrganizationMemberDetails",
                "kind": "LinkedField",
                "name": "organizationMember",
                "plural": false,
                "selections": (v4/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
        "args": (v5/*: any*/),
        "concreteType": "OrganizationMemberConnection",
        "kind": "LinkedField",
        "name": "organizationMembers",
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
                  (v1/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v3/*: any*/),
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
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "photoUrl",
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
        "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
        "args": (v5/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers",
        "kind": "LinkedHandle",
        "name": "organizationMembers"
      }
    ]
  },
  "params": {
    "cacheID": "8963f514f1aeff1b42d89a57b87b383d",
    "id": null,
    "metadata": {},
    "name": "teamMembersTab_refetchableFragment",
    "operationKind": "query",
    "text": "query teamMembersTab_refetchableFragment(\n  $bookingPeopleNameSearchText: String\n  $organizationId: String!\n  $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $teamId: String!\n) {\n  ...teamMembersTab_query\n}\n\nfragment organizationMemberSelector_query on Query {\n  organizationMemberSelectorPaginatedOrganizationMembers: organizationMembers(first: 20, where: {organizationId: $organizationId, nameContains: $bookingPeopleNameSearchText}, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment teamMemberCard_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    canModify\n    members {\n      id\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n    }\n  }\n}\n\nfragment teamMembersTab_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    timezone\n    organization {\n      name\n    }\n    canModify\n    members {\n      id\n      customer {\n        uniqueId\n      }\n      organizationMember {\n        uniqueId\n      }\n    }\n  }\n  ...teamMemberCard_query\n  ...organizationMemberSelector_query\n}\n"
  }
};
})();

(node as any).hash = "5cc74900fa9bfa3d9c61c278d6b7bb9c";

export default node;
