/**
 * @generated SignedSource<<dbe45f55e719c8f652ecc8007cb02137>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InvitationStatus = "ACCEPTED" | "CANCELLED" | "EXPIRED" | "PENDING" | "REJECTED" | "%future added value";
export type RejectInvitationToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_rejectInvitationToJoinTeamMutation$variables = {
  input: RejectInvitationToJoinTeamInput;
};
export type notifications_rejectInvitationToJoinTeamMutation$data = {
  readonly rejectInvitationToJoinTeam: {
    readonly inviteCustomerToJoinTeam: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_rejectInvitationToJoinTeamMutation$rawResponse = {
  readonly rejectInvitationToJoinTeam: {
    readonly inviteCustomerToJoinTeam: {
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: InvitationStatus;
      };
    };
  };
};
export type notifications_rejectInvitationToJoinTeamMutation = {
  rawResponse: notifications_rejectInvitationToJoinTeamMutation$rawResponse;
  response: notifications_rejectInvitationToJoinTeamMutation$data;
  variables: notifications_rejectInvitationToJoinTeamMutation$variables;
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
    "name": "rejectInvitationToJoinTeam",
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "6ae926d5e6f78495626826099e00225c",
    "id": null,
    "metadata": {},
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_rejectInvitationToJoinTeamMutation(\n  $input: RejectInvitationToJoinTeamInput!\n) {\n  rejectInvitationToJoinTeam(input: $input) {\n    inviteCustomerToJoinTeam {\n      id\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "699bb812eed636f619bda87d1ad81b6e";

export default node;
