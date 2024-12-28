/**
 * @generated SignedSource<<efcca660b7f23fea942e644aa4d39b68>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type TeamMemberOrderField = "FamilyName" | "GivenName" | "MembershipType" | "MiddleName" | "Name" | "Status" | "%future added value";
export type TeamMemberOrderInput = {
  direction: OrderDirection;
  field: TeamMemberOrderField;
};
export type teamMembersTab_teamMembers_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  peopleNameSearchText?: string | null | undefined;
  teamId: string;
  teamMembersSortingValues?: ReadonlyArray<TeamMemberOrderInput> | null | undefined;
};
export type teamMembersTab_teamMembers_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"teamMembersTab_teamMembers_query">;
};
export type teamMembersTab_teamMembers_refetchableFragment = {
  response: teamMembersTab_teamMembers_refetchableFragment$data;
  variables: teamMembersTab_teamMembers_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
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
    "name": "peopleNameSearchText"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamMembersSortingValues"
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
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "name",
      "storageKey": null
    },
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamMembersTab_teamMembers_refetchableFragment",
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
        "name": "teamMembersTab_teamMembers_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamMembersTab_teamMembers_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberConnection",
        "kind": "LinkedField",
        "name": "teamMembers",
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
                    "kind": "ScalarField",
                    "name": "id",
                    "storageKey": null
                  },
                  (v2/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "organizationMember",
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
        "key": "teamMembersTab_teamMembers",
        "kind": "LinkedHandle",
        "name": "teamMembers"
      }
    ]
  },
  "params": {
    "cacheID": "70ff96341b64cc05e78bfbcf6e910cd0",
    "id": null,
    "metadata": {},
    "name": "teamMembersTab_teamMembers_refetchableFragment",
    "operationKind": "query",
    "text": "query teamMembersTab_teamMembers_refetchableFragment(\n  $count: Int = 50\n  $cursor: String\n  $peopleNameSearchText: String\n  $teamId: String!\n  $teamMembersSortingValues: [TeamMemberOrderInput!]\n) {\n  ...teamMembersTab_teamMembers_query_1G22uz\n}\n\nfragment teamMemberCard_TeamMemberDetails on TeamMemberDetails {\n  id\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n  organizationMember {\n    customer {\n      name\n      givenName\n      middleName\n      familyName\n      photoUrl\n    }\n  }\n}\n\nfragment teamMembersTab_teamMembers_query_1G22uz on Query {\n  teamMembers(first: $count, after: $cursor, where: {teamId: $teamId, nameContains: $peopleNameSearchText}, orderBy: $teamMembersSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...teamMemberCard_TeamMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2e77cd2f8f60b5bc9b6827c774355ef7";

export default node;
