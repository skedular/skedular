/**
 * @generated SignedSource<<9df8e9e4949cd685348f1de212254b7e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AcceptInvitationToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation$variables = {
  input: AcceptInvitationToJoinOrganizationInput;
};
export type invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation$data = {
  readonly acceptInvitationToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation = {
  response: invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation$data;
  variables: invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation$variables;
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
    "concreteType": "AcceptInvitationToJoinOrganizationPayload",
    "kind": "LinkedField",
    "name": "acceptInvitationToJoinOrganization",
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
    "name": "invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "ea51daed98f0cf52f563b5d00ea12c6d",
    "id": null,
    "metadata": {},
    "name": "invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation invitationToJoinOrganizationNotificationCard_acceptInvitationToJoinOrganizationMutation(\n  $input: AcceptInvitationToJoinOrganizationInput!\n) {\n  acceptInvitationToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "3f7ef070c77e37f87efd878bfd465b9d";

export default node;
