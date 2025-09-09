/**
 * @generated SignedSource<<e1a2c12d81df60218901d9ffdac7828b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type TeamMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type TeamMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type ChangeTeamMembersStatusInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
  status: TeamMemberStatus;
};
export type organizationTeam_changeTeamMembersStatusMutation$variables = {
  input: ChangeTeamMembersStatusInput;
};
export type organizationTeam_changeTeamMembersStatusMutation$data = {
  readonly changeTeamMembersStatus: {
    readonly members: ReadonlyArray<{
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
      readonly role: TeamMemberRole | null | undefined;
      readonly status: TeamMemberStatus;
    }>;
  };
};
export type organizationTeam_changeTeamMembersStatusMutation = {
  response: organizationTeam_changeTeamMembersStatusMutation$data;
  variables: organizationTeam_changeTeamMembersStatusMutation$variables;
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
    "concreteType": "TeamMembersDetailsPayload",
    "kind": "LinkedField",
    "name": "changeTeamMembersStatus",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamMemberDetails",
        "kind": "LinkedField",
        "name": "members",
        "plural": true,
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
    "name": "organizationTeam_changeTeamMembersStatusMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeam_changeTeamMembersStatusMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "9704a7f4f4b14f9fdde728d3cc1b7f9f",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_changeTeamMembersStatusMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_changeTeamMembersStatusMutation(\n  $input: ChangeTeamMembersStatusInput!\n) {\n  changeTeamMembersStatus(input: $input) {\n    members {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "22b646377db959a5bb4bcfa8969d7d1d";

export default node;
