/**
 * @generated SignedSource<<d332f07082a815082f73d002c8069df5>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type OrganizationMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationUsers_organizationMembers_query$data = {
  readonly organization: {
    readonly members: {
      readonly __id: string;
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly customer: {
            readonly email: string | null | undefined;
            readonly familyName: string | null | undefined;
            readonly givenName: string | null | undefined;
            readonly id: string;
            readonly middleName: string | null | undefined;
            readonly name: string | null | undefined;
            readonly phoneNumber: string | null | undefined;
            readonly photoUrl: string | null | undefined;
          };
          readonly id: string;
          readonly role: {
            readonly name: string;
            readonly type: OrganizationMemberRole;
          };
          readonly status: {
            readonly name: string;
            readonly type: OrganizationMemberStatus;
          };
        };
      }>;
      readonly pageInfo: {
        readonly endCursor: string | null | undefined;
        readonly hasNextPage: boolean;
        readonly hasPreviousPage: boolean;
        readonly startCursor: string | null | undefined;
      };
      readonly totalCount: number;
    };
  } | null | undefined;
  readonly " $fragmentType": "organizationUsers_organizationMembers_query";
};
export type organizationUsers_organizationMembers_query$key = {
  readonly " $data"?: organizationUsers_organizationMembers_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationUsers_organizationMembers_query">;
};

import organizationUsers_organizationUsers_refetchableFragment_graphql from './organizationUsers_organizationUsers_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "organization",
  "members"
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
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v2/*:: as any*/)
];
return {
  "argumentDefinitions": [
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
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    },
    {
      "kind": "RootArgument",
      "name": "peopleNameSearchText"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": null,
        "cursor": null,
        "direction": "bidirectional",
        "path": (v0/*:: as any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": {
          "count": "last",
          "cursor": "before"
        },
        "path": (v0/*:: as any*/)
      },
      "fragmentPathInResult": [],
      "operation": organizationUsers_organizationUsers_refetchableFragment_graphql
    }
  },
  "name": "organizationUsers_organizationMembers_query",
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
          "alias": "members",
          "args": [
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
          "concreteType": "ConnectionOfOrganizationMemberEdge",
          "kind": "LinkedField",
          "name": "__organizationMembers_members_connection",
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
                    (v1/*:: as any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "CustomerDetails",
                      "kind": "LinkedField",
                      "name": "customer",
                      "plural": false,
                      "selections": [
                        (v1/*:: as any*/),
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
                      "selections": (v3/*:: as any*/),
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationMemberRoleDetails",
                      "kind": "LinkedField",
                      "name": "role",
                      "plural": false,
                      "selections": (v3/*:: as any*/),
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
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "e78e93ad5d7e029f3223ab3991df1fc0";

export default node;
