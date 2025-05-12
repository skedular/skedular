/**
 * @generated SignedSource<<18eb23e4f2187d8d2b04e4bfd45153c9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type TeamMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type TeamMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type ChangeTeamMemberRoleInput = {
  clientMutationId?: string | null | undefined;
  id: string;
  role: TeamMemberRole;
};
export type organizationTeam_changeTeamMemberRoleMutation$variables = {
  input: ChangeTeamMemberRoleInput;
};
export type organizationTeam_changeTeamMemberRoleMutation$data = {
  readonly changeTeamMemberRole: {
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
      readonly role: TeamMemberRole | null | undefined;
      readonly status: TeamMemberStatus;
    } | null | undefined;
  } | null | undefined;
};
export type organizationTeam_changeTeamMemberRoleMutation$rawResponse = {
  readonly changeTeamMemberRole: {
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
      readonly role: TeamMemberRole | null | undefined;
      readonly status: TeamMemberStatus;
    } | null | undefined;
  } | null | undefined;
};
export type organizationTeam_changeTeamMemberRoleMutation = {
  rawResponse: organizationTeam_changeTeamMemberRoleMutation$rawResponse;
  response: organizationTeam_changeTeamMemberRoleMutation$data;
  variables: organizationTeam_changeTeamMemberRoleMutation$variables;
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
    "concreteType": "TeamMemberDetailsPayload",
    "kind": "LinkedField",
    "name": "changeTeamMemberRole",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamMemberDetails",
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
            "concreteType": "Team_CustomerDetails",
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
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "6aaa34f198ba98622233f2be67446dfd",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_changeTeamMemberRoleMutation(\n  $input: ChangeTeamMemberRoleInput!\n) {\n  changeTeamMemberRole(input: $input) {\n    member {\n      id\n      customer {\n        uniqueId\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d2d7935049d3c9b16233dfc1b48750bf";

export default node;
