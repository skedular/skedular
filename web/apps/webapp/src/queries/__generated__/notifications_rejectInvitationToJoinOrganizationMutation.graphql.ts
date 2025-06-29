/**
 * @generated SignedSource<<41082007e8d59ffd55ce1f5093e05e96>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RejectInvitationToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_rejectInvitationToJoinOrganizationMutation$variables = {
  input: RejectInvitationToJoinOrganizationInput;
};
export type notifications_rejectInvitationToJoinOrganizationMutation$data = {
  readonly rejectInvitationToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type notifications_rejectInvitationToJoinOrganizationMutation = {
  response: notifications_rejectInvitationToJoinOrganizationMutation$data;
  variables: notifications_rejectInvitationToJoinOrganizationMutation$variables;
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
    "concreteType": "RejectInvitationToJoinOrganizationPayload",
    "kind": "LinkedField",
    "name": "rejectInvitationToJoinOrganization",
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
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "32e0d8420ff908baf068c82641be1360",
    "id": null,
    "metadata": {},
    "name": "notifications_rejectInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_rejectInvitationToJoinOrganizationMutation(\n  $input: RejectInvitationToJoinOrganizationInput!\n) {\n  rejectInvitationToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "952f65b17c9e960d688d3290a139ffa5";

export default node;
