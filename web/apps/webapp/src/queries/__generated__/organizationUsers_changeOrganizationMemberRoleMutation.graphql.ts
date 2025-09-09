/**
 * @generated SignedSource<<59be68d0d739bcefbc65edd612a16a85>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type OrganizationMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
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
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      };
      readonly id: string;
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
    } | null | undefined;
  };
};
export type organizationUsers_changeOrganizationMemberRoleMutation$rawResponse = {
  readonly changeOrganizationMemberRole: {
    readonly member: {
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
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
    } | null | undefined;
  };
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v1/*: any*/),
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
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "73a776c5a1cde68a48bf2b6c53fb46cf",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUsers_changeOrganizationMemberRoleMutation(\n  $input: ChangeOrganizationMemberRoleInput!\n) {\n  changeOrganizationMemberRole(input: $input) {\n    member {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9b806ef35f4f574e8cce0685e5d45c0f";

export default node;
