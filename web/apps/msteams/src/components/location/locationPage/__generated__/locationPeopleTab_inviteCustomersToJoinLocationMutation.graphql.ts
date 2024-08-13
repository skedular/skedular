/**
 * @generated SignedSource<<150d38bdba2dd51e4dc93937508747b0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type InviteCustomersToJoinLocationInput = {
  clientMutationId?: string | null | undefined;
  emails: ReadonlyArray<string>;
  locationId: string;
};
export type locationPeopleTab_inviteCustomersToJoinLocationMutation$variables = {
  input: InviteCustomersToJoinLocationInput;
};
export type locationPeopleTab_inviteCustomersToJoinLocationMutation$data = {
  readonly inviteCustomersToJoinLocation: {
    readonly clientMutationId: string | null | undefined;
  } | null | undefined;
};
export type locationPeopleTab_inviteCustomersToJoinLocationMutation = {
  response: locationPeopleTab_inviteCustomersToJoinLocationMutation$data;
  variables: locationPeopleTab_inviteCustomersToJoinLocationMutation$variables;
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
    "name": "locationPeopleTab_inviteCustomersToJoinLocationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationPeopleTab_inviteCustomersToJoinLocationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "34d40cb579d1c07b5e34afb2b404fff1",
    "id": null,
    "metadata": {},
    "name": "locationPeopleTab_inviteCustomersToJoinLocationMutation",
    "operationKind": "mutation",
    "text": "mutation locationPeopleTab_inviteCustomersToJoinLocationMutation(\n  $input: InviteCustomersToJoinLocationInput!\n) {\n  inviteCustomersToJoinLocation(input: $input) {\n    clientMutationId\n  }\n}\n"
  }
};
})();

(node as any).hash = "7063bbaa2391a32551bbdd16f2fd468a";

export default node;
