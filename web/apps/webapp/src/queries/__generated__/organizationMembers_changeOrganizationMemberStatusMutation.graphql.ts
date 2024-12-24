/**
 * @generated SignedSource<<35503718b73a4c447e833c2960606740>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberStatus = "Active" | "Inactive" | "%future added value";
export type ChangeOrganizationMemberStatusInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
  status: OrganizationMemberStatus;
};
export type organizationMembers_changeOrganizationMemberStatusMutation$variables = {
  input: ChangeOrganizationMemberStatusInput;
};
export type organizationMembers_changeOrganizationMemberStatusMutation$data = {
  readonly changeOrganizationMemberStatus: {
    readonly members: ReadonlyArray<{
      readonly customer: {
        readonly email: string | null | undefined;
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly id: string;
      readonly status: OrganizationMemberStatus;
    }>;
  } | null | undefined;
};
export type organizationMembers_changeOrganizationMemberStatusMutation = {
  response: organizationMembers_changeOrganizationMemberStatusMutation$data;
  variables: organizationMembers_changeOrganizationMemberStatusMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationMembersDetailsPayload",
    "kind": "LinkedField",
    "name": "changeOrganizationMemberStatus",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberDetails",
        "kind": "LinkedField",
        "name": "members",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
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
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
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
            "kind": "ScalarField",
            "name": "status",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMembers_changeOrganizationMemberStatusMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembers_changeOrganizationMemberStatusMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5d299763c60401c861cda2b07f3bbfbe",
    "id": null,
    "metadata": {},
    "name": "organizationMembers_changeOrganizationMemberStatusMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembers_changeOrganizationMemberStatusMutation(\n  $input: ChangeOrganizationMemberStatusInput!\n) {\n  changeOrganizationMemberStatus(input: $input) {\n    members {\n      id\n      customer {\n        uniqueId\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "30e41c5da2cc4f00cda46d5f518cd8c9";

export default node;
