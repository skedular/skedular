/**
 * @generated SignedSource<<08d4a451141955ca0b255202eac80d14>>
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
export type teamPeopleTab_inviteCustomersToJoinTeamMutation$variables = {
  input: InviteCustomersToJoinTeamInput;
};
export type teamPeopleTab_inviteCustomersToJoinTeamMutation$data = {
  readonly inviteCustomersToJoinTeam: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type teamPeopleTab_inviteCustomersToJoinTeamMutation = {
  response: teamPeopleTab_inviteCustomersToJoinTeamMutation$data;
  variables: teamPeopleTab_inviteCustomersToJoinTeamMutation$variables;
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
    "name": "teamPeopleTab_inviteCustomersToJoinTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamPeopleTab_inviteCustomersToJoinTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "9c5fa543d07ee14f5e13c0f32825c4c5",
    "id": null,
    "metadata": {},
    "name": "teamPeopleTab_inviteCustomersToJoinTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleTab_inviteCustomersToJoinTeamMutation(\n  $input: InviteCustomersToJoinTeamInput!\n) {\n  inviteCustomersToJoinTeam(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "a8dadfcfc6e8a6d4aef1943f941e1f18";

export default node;
