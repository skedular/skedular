/**
 * @generated SignedSource<<7d86751ab7db1403d0b75072ca8faf24>>
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
export type notifications_acceptInvitationToJoinTeamMutation$variables = {
  input: AcceptInvitationToJoinTeamInput;
};
export type notifications_acceptInvitationToJoinTeamMutation$data = {
  readonly acceptInvitationToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type notifications_acceptInvitationToJoinTeamMutation = {
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
    "cacheID": "223c6c960886e655226ad600c4e74d64",
    "id": null,
    "metadata": {},
    "name": "notifications_acceptInvitationToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_acceptInvitationToJoinTeamMutation(\n  $input: AcceptInvitationToJoinTeamInput!\n) {\n  acceptInvitationToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "7ba4849fa96e6a58cf43fbe9d9e6134e";

export default node;
