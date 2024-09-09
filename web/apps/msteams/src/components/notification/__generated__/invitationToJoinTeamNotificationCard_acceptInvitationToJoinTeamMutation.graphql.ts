/**
 * @generated SignedSource<<89ff78f6e80b65ac2746ce6520398146>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AcceptInvitationToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation$variables = {
  input: AcceptInvitationToJoinTeamInput;
};
export type invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation$data = {
  readonly acceptInvitationToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation = {
  response: invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation$data;
  variables: invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation$variables;
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
    "concreteType": "AcceptInvitationToJoinTeamPayload",
    "kind": "LinkedField",
    "name": "acceptInvitationToJoinTeam",
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
    "name": "invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "63bf2ed90006348301938cc75ccf2807",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinTeamNotificationCard_acceptInvitationToJoinTeamMutation(\n  $input: AcceptInvitationToJoinTeamInput!\n) {\n  acceptInvitationToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "5973eda6eb359491fb43c3ea096b281d";

export default node;
