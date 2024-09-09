/**
 * @generated SignedSource<<6367c563858eeac951fbfb932c18f867>>
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
export type invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation$variables = {
  input: RejectInvitationToJoinOrganizationInput;
};
export type invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation$data = {
  readonly rejectInvitationToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation = {
  response: invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation$data;
  variables: invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation$variables;
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
    "name": "invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "632226f0e887d0a1098bf0e1d82f06c5",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinOrganizationNotificationCard_rejectInvitationToJoinOrganizationMutation(\n  $input: RejectInvitationToJoinOrganizationInput!\n) {\n  rejectInvitationToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "43fbbfb84d02820a284edfa1a04f600b";

export default node;
