/**
 * @generated SignedSource<<0dee60ffdc3a360f73ae7e83f9ff1a0f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AcceptInvitationToJoinLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation$variables = {
  input: AcceptInvitationToJoinLocationInput;
};
export type invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation$data = {
  readonly acceptInvitationToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation = {
  response: invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation$data;
  variables: invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation$variables;
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
    "concreteType": "AcceptInvitationToJoinLocationPayload",
    "kind": "LinkedField",
    "name": "acceptInvitationToJoinLocation",
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
    "name": "invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "28c0d5fa14566eeadc2d3a10886338b6",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinLocationNotificationCard_acceptInvitationToJoinLocationMutation(\n  $input: AcceptInvitationToJoinLocationInput!\n) {\n  acceptInvitationToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "28bd3b8a604b60b5903f1c4d1a573b5b";

export default node;
