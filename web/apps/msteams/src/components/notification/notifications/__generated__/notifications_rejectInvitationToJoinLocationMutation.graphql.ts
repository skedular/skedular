/**
 * @generated SignedSource<<89faf937cbe8579ee81f746af47dfd17>>
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
export type notifications_rejectInvitationToJoinLocationMutation$variables = {
  input: RejectInvitationToJoinLocationInput;
};
export type notifications_rejectInvitationToJoinLocationMutation$data = {
  readonly rejectInvitationToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type notifications_rejectInvitationToJoinLocationMutation = {
  response: notifications_rejectInvitationToJoinLocationMutation$data;
  variables: notifications_rejectInvitationToJoinLocationMutation$variables;
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
    "name": "notifications_rejectInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_rejectInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2365e60e98123df3389dd0c9eb3a6441",
    "id": null,
    "metadata": {},
    "name": "notifications_rejectInvitationToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_rejectInvitationToJoinLocationMutation(\n  $input: RejectInvitationToJoinLocationInput!\n) {\n  rejectInvitationToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "2db0d8de9c01be835bae6ff0226777f8";

export default node;
