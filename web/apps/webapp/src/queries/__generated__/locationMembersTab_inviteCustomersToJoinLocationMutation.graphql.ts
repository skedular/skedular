/**
 * @generated SignedSource<<cd95f99fa611691e88180961a864da49>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type InviteCustomersToJoinLocationInput = {
  clientMutationId?: string | null | undefined;
  emails: ReadonlyArray<string>;
  locationId: string;
};
export type locationMembersTab_inviteCustomersToJoinLocationMutation$variables = {
  input: InviteCustomersToJoinLocationInput;
};
export type locationMembersTab_inviteCustomersToJoinLocationMutation$data = {
  readonly inviteCustomersToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type locationMembersTab_inviteCustomersToJoinLocationMutation = {
  response: locationMembersTab_inviteCustomersToJoinLocationMutation$data;
  variables: locationMembersTab_inviteCustomersToJoinLocationMutation$variables;
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
    "concreteType": "InviteCustomersToJoinLocationPayload",
    "kind": "LinkedField",
    "name": "inviteCustomersToJoinLocation",
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
    "name": "locationMembersTab_inviteCustomersToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationMembersTab_inviteCustomersToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "4522972c5ca028489d9f96a82559d35b",
    "id": null,
    "metadata": {},
    "name": "locationMembersTab_inviteCustomersToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationMembersTab_inviteCustomersToJoinLocationMutation(\n  $input: InviteCustomersToJoinLocationInput!\n) {\n  inviteCustomersToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "2fb0a4d4c5902d7629577d35a06d7aad";

export default node;
