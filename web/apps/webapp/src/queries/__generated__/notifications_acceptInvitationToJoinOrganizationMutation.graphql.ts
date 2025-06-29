/**
 * @generated SignedSource<<95f6429b7a32e5f1c0cc791ca17af3d3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AcceptInvitationToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type notifications_acceptInvitationToJoinOrganizationMutation$variables = {
  input: AcceptInvitationToJoinOrganizationInput;
};
export type notifications_acceptInvitationToJoinOrganizationMutation$data = {
  readonly acceptInvitationToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type notifications_acceptInvitationToJoinOrganizationMutation = {
  response: notifications_acceptInvitationToJoinOrganizationMutation$data;
  variables: notifications_acceptInvitationToJoinOrganizationMutation$variables;
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
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "e9ad1756f67512cd4c758dd8d5fa94ad",
    "id": null,
    "metadata": {},
    "name": "notifications_acceptInvitationToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation notifications_acceptInvitationToJoinOrganizationMutation(\n  $input: AcceptInvitationToJoinOrganizationInput!\n) {\n  acceptInvitationToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "b466e0822bd75f2b1a191232af95988e";

export default node;
