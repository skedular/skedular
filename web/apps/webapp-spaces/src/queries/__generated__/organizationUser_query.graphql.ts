/**
 * @generated SignedSource<<00aba82e6e03aafdcb2ee2d02f6bf9f1>>
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*:: as any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "customerId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
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
        (v0/*:: as any*/)
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
        (v0/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "email",
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
        (v1/*:: as any*/),
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
        {
          "alias": null,
          "args": null,
          "concreteType": "PersonalInformationVisibilityDetails",
          "kind": "LinkedField",
          "name": "personalInformationVisibility",
          "plural": false,
          "selections": (v2/*:: as any*/),
          "storageKey": null
        }
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
                {
                  "kind": "Variable",
                  "name": "customerId",
                  "variableName": "customerId"
                }
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
                    (v0/*:: as any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "OrganizationMemberStatusDetails",
                      "kind": "LinkedField",
                      "name": "status",
                      "plural": false,
                      "selections": (v2/*:: as any*/),
                      "storageKey": null
                    }
                  ],
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

(node as any).hash = "1ae19b1e84c5920bb6cb9e2fda03a54d";

export default node;
