/**
 * @generated SignedSource<<2f1e2873f6baf34b2dbf7a4a2100592e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AcceptInvitationToJoinLocationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_acceptInvitationToJoinLocationMutation$variables = {
  input: AcceptInvitationToJoinLocationInput;
};
export type notifications_acceptInvitationToJoinLocationMutation$data = {
  readonly acceptInvitationToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type notifications_acceptInvitationToJoinLocationMutation = {
  response: notifications_acceptInvitationToJoinLocationMutation$data;
  variables: notifications_acceptInvitationToJoinLocationMutation$variables;
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
    "name": "notifications_acceptInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_acceptInvitationToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e4ea7b7093ef421090bf86b540f88fa0",
    "id": null,
    "metadata": {},
    "name": "notifications_acceptInvitationToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_acceptInvitationToJoinLocationMutation(\n  $input: AcceptInvitationToJoinLocationInput!\n) {\n  acceptInvitationToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "29fa87cefb5d3b765f6a1e421c3e2a99";

export default node;
