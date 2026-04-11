/**
 * @generated SignedSource<<8a99fb30db7cbf28ba902ce52ee8ba91>>
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
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      };
      readonly id: string;
      readonly role: {
        readonly name: string;
        readonly type: TeamMemberRole;
      };
      readonly status: {
        readonly name: string;
        readonly type: TeamMemberStatus;
      };
    } | null | undefined;
  };
};
export type organizationTeam_changeTeamMemberRoleMutation$rawResponse = {
  readonly changeTeamMemberRole: {
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
        readonly type: TeamMemberRole;
      };
      readonly status: {
        readonly name: string;
        readonly type: TeamMemberStatus;
      };
    } | null | undefined;
  };
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
            "concreteType": "TeamMemberStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberRoleDetails",
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
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "a5f0cdb25602c77ed54b073557e578b8",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_changeTeamMemberRoleMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_changeTeamMemberRoleMutation(\n  $input: ChangeTeamMemberRoleInput!\n) {\n  changeTeamMemberRole(input: $input) {\n    member {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status {\n        type\n        name\n      }\n      role {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "405fd9a11dcac6f6f7ac18e2301436e5";

export default node;
