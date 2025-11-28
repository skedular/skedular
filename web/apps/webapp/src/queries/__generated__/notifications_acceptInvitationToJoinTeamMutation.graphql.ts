/**
 * @generated SignedSource<<768aa2b78b652dea34279fb2d7b1cf0c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InvitationStatus = "ACCEPTED" | "CANCELLED" | "EXPIRED" | "PENDING" | "REJECTED" | "%future added value";
export type AcceptInvitationToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_acceptInvitationToJoinTeamMutation$variables = {
  input: AcceptInvitationToJoinTeamInput;
};
export type notifications_acceptInvitationToJoinTeamMutation$data = {
  readonly acceptInvitationToJoinTeam: {
    readonly inviteCustomerToJoinTeam: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_acceptInvitationToJoinTeamMutation$rawResponse = {
  readonly acceptInvitationToJoinTeam: {
    readonly inviteCustomerToJoinTeam: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_acceptInvitationToJoinTeamMutation = {
  rawResponse: notifications_acceptInvitationToJoinTeamMutation$rawResponse;
  response: notifications_acceptInvitationToJoinTeamMutation$data;
  variables: notifications_acceptInvitationToJoinTeamMutation$variables;
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
    "concreteType": "InvitationToJoinTeamPayload",
    "kind": "LinkedField",
    "name": "acceptInvitationToJoinTeam",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "InviteCustomerToJoinTeamDetails",
        "kind": "LinkedField",
        "name": "inviteCustomerToJoinTeam",
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
            "concreteType": "TeamInvitationStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
            ],
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
    "name": "notifications_acceptInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_acceptInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "7de2c4011977150f45939499e1f1eb53",
    "id": null,
    "metadata": {},
    "name": "notifications_acceptInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_acceptInvitationToJoinTeamMutation(\n  $input: AcceptInvitationToJoinTeamInput!\n) {\n  acceptInvitationToJoinTeam(input: $input) {\n    inviteCustomerToJoinTeam {\n      id\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f526d248890c90975ecabd93bcdb4769";

export default node;
