/**
 * @generated SignedSource<<ecb5fb7da0d5c1bdc6073e2c55800231>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RejectInvitationToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_rejectInvitationToJoinTeamMutation$variables = {
  input: RejectInvitationToJoinTeamInput;
};
export type notifications_rejectInvitationToJoinTeamMutation$data = {
  readonly rejectInvitationToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type notifications_rejectInvitationToJoinTeamMutation = {
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
    "concreteType": "RejectInvitationToJoinTeamPayload",
    "kind": "LinkedField",
    "name": "rejectInvitationToJoinTeam",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "clientMutationId",
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
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "420c25a6d62344ddf7246584d43e81a8",
    "id": null,
    "metadata": {},
    "name": "notifications_rejectInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_rejectInvitationToJoinTeamMutation(\n  $input: RejectInvitationToJoinTeamInput!\n) {\n  rejectInvitationToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "2262794790f5a881bfccbeccf6193da1";

export default node;
