/**
 * @generated SignedSource<<ba953dd86f26cf66033a82012b233ee2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type RejectInvitationToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation$variables = {
  input: RejectInvitationToJoinTeamInput;
};
export type invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation$data = {
  readonly rejectInvitationToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation = {
  response: invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation$data;
  variables: invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation$variables;
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
    "name": "invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "207cb74638baa03ab64a3158329ad337",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinTeamNotificationCard_rejectInvitationToJoinTeamMutation(\n  $input: RejectInvitationToJoinTeamInput!\n) {\n  rejectInvitationToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "c1b127baa619c856e1d582a703144a5b";

export default node;
