/**
 * @generated SignedSource<<fdeb3fb653c442e46aea2d3bf74d3e1d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationUsers_organizationUsers_refetchableFragment$variables = {
  before?: string | null | undefined;
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  last?: number | null | undefined;
  organizationCustomDomain?: string | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type organizationUsers_organizationUsers_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationUsers_organizationMembers_query">;
};
export type organizationUsers_organizationUsers_refetchableFragment = {
  response: organizationUsers_organizationUsers_refetchableFragment$data;
  variables: organizationUsers_organizationUsers_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "before"
  },
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
    "name": "last"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "peopleNameSearchText"
  }
],
v1 = {
  "kind": "Variable",
  "name": "before",
  "variableName": "before"
},
v2 = {
  "kind": "Variable",
  "name": "last",
  "variableName": "last"
},
v3 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  (v1/*:: as any*/),
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
  (v2/*:: as any*/),
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
  "name": "name",
  "storageKey": null
},
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v5/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationUsers_organizationUsers_refetchableFragment",
    "selections": [
      {
        "args": [
          (v1/*:: as any*/),
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          },
          (v2/*:: as any*/)
        ],
        "kind": "FragmentSpread",
        "name": "organizationUsers_organizationMembers_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationUsers_organizationUsers_refetchableFragment",
    "selections": [
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
            "args": (v3/*:: as any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
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
                    "name": "startCursor",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasNextPage",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasPreviousPage",
                    "storageKey": null
                  }
                ],
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
                      (v4/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v4/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "email",
                            "storageKey": null
                          },
                          (v5/*:: as any*/),
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
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationMemberStatusDetails",
                        "kind": "LinkedField",
                        "name": "status",
                        "plural": false,
                        "selections": (v6/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationMemberRoleDetails",
                        "kind": "LinkedField",
                        "name": "role",
                        "plural": false,
                        "selections": (v6/*:: as any*/),
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
            "args": (v3/*:: as any*/),
            "filters": [
              "where"
            ],
            "handle": "connection",
            "key": "organizationMembers_members",
            "kind": "LinkedHandle",
            "name": "members"
          },
          (v4/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "09963fd8cea74a1b2dabc2211461a8cd",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_organizationUsers_refetchableFragment",
    "operationKind": "query",
    "text": "query organizationUsers_organizationUsers_refetchableFragment(\n  $before: String\n  $count: Int = null\n  $cursor: String\n  $last: Int\n  $organizationCustomDomain: String\n  $peopleNameSearchText: String\n) {\n  ...organizationUsers_organizationMembers_query_2aiyqj\n}\n\nfragment organizationUsers_organizationMembers_query_2aiyqj on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(first: $count, after: $cursor, last: $last, before: $before, where: {nameContains: $peopleNameSearchText}) {\n      totalCount\n      pageInfo {\n        endCursor\n        startCursor\n        hasNextPage\n        hasPreviousPage\n      }\n      edges {\n        node {\n          id\n          customer {\n            id\n            email\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n            phoneNumber\n          }\n          status {\n            type\n            name\n          }\n          role {\n            type\n            name\n          }\n          __typename\n        }\n        cursor\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "e78e93ad5d7e029f3223ab3991df1fc0";

export default node;
