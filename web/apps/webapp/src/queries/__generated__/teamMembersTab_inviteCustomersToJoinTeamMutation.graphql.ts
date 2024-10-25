/**
 * @generated SignedSource<<9e4a4a144dd99d9dd5128e37600c7ea1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InviteCustomersToJoinTeamInput = {
  clientMutationId?: string | null | undefined;
  emails: ReadonlyArray<string>;
  teamId: string;
};
export type teamMembersTab_inviteCustomersToJoinTeamMutation$variables = {
  input: InviteCustomersToJoinTeamInput;
};
export type teamMembersTab_inviteCustomersToJoinTeamMutation$data = {
  readonly inviteCustomersToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type teamMembersTab_inviteCustomersToJoinTeamMutation = {
  response: teamMembersTab_inviteCustomersToJoinTeamMutation$data;
  variables: teamMembersTab_inviteCustomersToJoinTeamMutation$variables;
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
    "concreteType": "InviteCustomersToJoinTeamPayload",
    "kind": "LinkedField",
    "name": "inviteCustomersToJoinTeam",
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
    "name": "teamMembersTab_inviteCustomersToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamMembersTab_inviteCustomersToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "ed988960fac518244a35a79b3407bd4f",
    "id": null,
    "metadata": {},
    "name": "teamMembersTab_inviteCustomersToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamMembersTab_inviteCustomersToJoinTeamMutation(\n  $input: InviteCustomersToJoinTeamInput!\n) {\n  inviteCustomersToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "0ccd9fa99f9728fbf2be5b277339e11f";

export default node;
