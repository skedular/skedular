/**
 * @generated SignedSource<<a5f5211de2f57a104a381cc915d7e3c1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InviteCustomersToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  emails: ReadonlyArray<string>;
  organizationId?: string | null | undefined;
  organizationUniqueAlphanumericName?: string | null | undefined;
};
export type invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation$variables = {
  input: InviteCustomersToJoinOrganizationInput;
};
export type invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation$data = {
  readonly inviteCustomersToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  };
};
export type invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation = {
  response: invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation$data;
  variables: invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation$variables;
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
    "concreteType": "InvitationsToJoinOrganizationPayload",
    "kind": "LinkedField",
    "name": "inviteCustomersToJoinOrganization",
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
    "name": "invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "849d823c6df2300ce0b93f67ff5e0462",
    "id": null,
    "metadata": {},
    "name": "invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation invitePeopleToJoinOrganizationDialog_inviteCustomersToJoinOrganizationMutation(\n  $input: InviteCustomersToJoinOrganizationInput!\n) {\n  inviteCustomersToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "49525060ea6e474f7011e3dc6bddaf89";

export default node;
