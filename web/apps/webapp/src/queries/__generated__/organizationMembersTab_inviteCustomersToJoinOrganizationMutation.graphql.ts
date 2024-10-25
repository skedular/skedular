/**
 * @generated SignedSource<<7b7e056d870a5dc803129ee8b1f8fe34>>
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
  organizationId: string;
};
export type organizationMembersTab_inviteCustomersToJoinOrganizationMutation$variables = {
  input: InviteCustomersToJoinOrganizationInput;
};
export type organizationMembersTab_inviteCustomersToJoinOrganizationMutation$data = {
  readonly inviteCustomersToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationMembersTab_inviteCustomersToJoinOrganizationMutation = {
  response: organizationMembersTab_inviteCustomersToJoinOrganizationMutation$data;
  variables: organizationMembersTab_inviteCustomersToJoinOrganizationMutation$variables;
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
    "concreteType": "InviteCustomersToJoinOrganizationPayload",
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
    "name": "organizationMembersTab_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembersTab_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "04741ccd4e1fb544181f465589f7954f",
    "id": null,
    "metadata": {},
    "name": "organizationMembersTab_inviteCustomersToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembersTab_inviteCustomersToJoinOrganizationMutation(\n  $input: InviteCustomersToJoinOrganizationInput!\n) {\n  inviteCustomersToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "3e69d80dfc93b6bceab261d6a38e12c3";

export default node;
