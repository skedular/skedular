/**
 * @generated SignedSource<<c65631def993ce2a475f3088ebde4b5e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RejectInvitationToJoinLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation$variables = {
  input: RejectInvitationToJoinLocationInput;
};
export type invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation$data = {
  readonly rejectInvitationToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation = {
  response: invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation$data;
  variables: invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation$variables;
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
    "concreteType": "RejectInvitationToJoinLocationPayload",
    "kind": "LinkedField",
    "name": "rejectInvitationToJoinLocation",
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
    "name": "invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f16bf799c1c352423386e952cc58258a",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinLocationNotificationCard_rejectInvitationToJoinLocationMutation(\n  $input: RejectInvitationToJoinLocationInput!\n) {\n  rejectInvitationToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "5cae8f7795185f24844f5e788766e5b6";

export default node;
