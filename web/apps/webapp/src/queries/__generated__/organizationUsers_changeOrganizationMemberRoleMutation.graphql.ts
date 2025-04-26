/**
 * @generated SignedSource<<be23519764323e69e894ccafa0e5b34a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberRole = "Administrator" | "Member" | "Owner" | "%future added value";
export type OrganizationMemberStatus = "Active" | "Inactive" | "%future added value";
export type ChangeOrganizationMemberRoleInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  role: OrganizationMemberRole;
};
export type organizationUsers_changeOrganizationMemberRoleMutation$variables = {
  input: ChangeOrganizationMemberRoleInput;
};
export type organizationUsers_changeOrganizationMemberRoleMutation$data = {
  readonly changeOrganizationMemberRole: {
    readonly member: {
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
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
    } | null | undefined;
  } | null | undefined;
};
export type organizationUsers_changeOrganizationMemberRoleMutation$rawResponse = {
  readonly changeOrganizationMemberRole: {
    readonly member: {
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
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
    } | null | undefined;
  } | null | undefined;
};
export type organizationUsers_changeOrganizationMemberRoleMutation = {
  rawResponse: organizationUsers_changeOrganizationMemberRoleMutation$rawResponse;
  response: organizationUsers_changeOrganizationMemberRoleMutation$data;
  variables: organizationUsers_changeOrganizationMemberRoleMutation$variables;
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
    "concreteType": "OrganizationMemberDetailsPayload",
    "kind": "LinkedField",
    "name": "changeOrganizationMemberRole",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberDetails",
        "kind": "LinkedField",
        "name": "member",
        "plural": false,
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
            "concreteType": "Organization_CustomerDetails",
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
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f582001c81a03dd112396d67d8834883",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUsers_changeOrganizationMemberRoleMutation(\n  $input: ChangeOrganizationMemberRoleInput!\n) {\n  changeOrganizationMemberRole(input: $input) {\n    member {\n      id\n      customer {\n        uniqueId\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1c73c2074692575a5875a8ed6281f884";

export default node;
