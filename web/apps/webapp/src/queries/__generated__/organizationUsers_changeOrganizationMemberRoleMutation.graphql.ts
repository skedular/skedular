/**
 * @generated SignedSource<<24e8f47dd8d4035ccf1eaf2506a5a46c>>
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
      readonly role: {
        readonly name: string;
        readonly type: OrganizationMemberRole;
      };
      readonly status: {
        readonly name: string;
        readonly type: OrganizationMemberStatus;
      };
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
      readonly role: {
        readonly name: string;
        readonly type: OrganizationMemberRole;
      };
      readonly status: {
        readonly name: string;
        readonly type: OrganizationMemberStatus;
      };
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
  (v2/*: any*/)
],
v4 = [
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
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberRoleDetails",
            "kind": "LinkedField",
            "name": "role",
            "plural": false,
            "selections": (v3/*: any*/),
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
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "4bbbc37bf528931a22da0497932d3907",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_changeOrganizationMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUsers_changeOrganizationMemberRoleMutation(\n  $input: ChangeOrganizationMemberRoleInput!\n) {\n  changeOrganizationMemberRole(input: $input) {\n    member {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status {\n        type\n        name\n      }\n      role {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "648dec9557fff579b2c5c7d1a4f7e462";

export default node;
