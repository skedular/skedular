/**
 * @generated SignedSource<<d7b03e203b00e83a013194144956dee8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type organizationUsers_rootQuery$variables = {
  organizationCustomDomain: string;
  peopleNameSearchText?: string | null | undefined;
};
export type organizationUsers_rootQuery$data = {
  readonly organization: {
    readonly canInvitePeople: boolean;
  } | null | undefined;
  readonly organizationMemberRoles: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationMemberRole;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUsers_organizationMembers_query">;
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
    "name": "organizationCustomDomain"
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
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canInvitePeople",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*:: as any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationMemberRoleDetails",
  "kind": "LinkedField",
  "name": "organizationMemberRoles",
  "plural": true,
  "selections": (v4/*:: as any*/),
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = [
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationUsers_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      (v5/*:: as any*/),
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationUsers_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": (v7/*:: as any*/),
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
                      (v6/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v6/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "email",
                            "storageKey": null
                          },
                          (v3/*:: as any*/),
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
                        "selections": (v4/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationMemberRoleDetails",
                        "kind": "LinkedField",
                        "name": "role",
                        "plural": false,
                        "selections": (v4/*:: as any*/),
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
            "args": (v7/*:: as any*/),
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
      (v5/*:: as any*/)
    ]
  },
  "params": {
    "cacheID": "9163b555350b24c017b155b537331759",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_rootQuery",
    "operationKind": "query",
    "text": "query organizationUsers_rootQuery(\n  $organizationCustomDomain: String!\n  $peopleNameSearchText: String\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    canInvitePeople\n    id\n  }\n  organizationMemberRoles {\n    type\n    name\n  }\n  ...organizationUsers_organizationMembers_query\n}\n\nfragment organizationUsers_organizationMembers_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $peopleNameSearchText}) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            email\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n            phoneNumber\n          }\n          status {\n            type\n            name\n          }\n          role {\n            type\n            name\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "d210562ce0a14de768ae89dc9d8a5c2f";

export default node;
