/**
 * @generated SignedSource<<cb7eb3410cdd4e2ac73b79705e6922b3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type InviteCustomersToJoinOrganizationInput = {
  clientMutationId?: string | null | undefined;
  emails: ReadonlyArray<string>;
  organizationId: string;
};
export type organizationPeopleTab_inviteCustomersToJoinOrganizationMutation$variables = {
  input: InviteCustomersToJoinOrganizationInput;
};
export type organizationPeopleTab_inviteCustomersToJoinOrganizationMutation$data = {
  readonly inviteCustomersToJoinOrganization: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type organizationPeopleTab_inviteCustomersToJoinOrganizationMutation = {
  response: organizationPeopleTab_inviteCustomersToJoinOrganizationMutation$data;
  variables: organizationPeopleTab_inviteCustomersToJoinOrganizationMutation$variables;
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
    "name": "organizationPeopleTab_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationPeopleTab_inviteCustomersToJoinOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5bc1694f345ab756229cc169152785ed",
    "id": null,
    "metadata": {},
    "name": "organizationPeopleTab_inviteCustomersToJoinOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationPeopleTab_inviteCustomersToJoinOrganizationMutation(\n  $input: InviteCustomersToJoinOrganizationInput!\n) {\n  inviteCustomersToJoinOrganization(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "89b4df87a1132438164d5912f82037e5";

export default node;
