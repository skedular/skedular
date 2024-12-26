/**
 * @generated SignedSource<<4794c690c1e3c58003abb89d5620c69b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationMembersInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationMembers_removeOrganizationMembersInputMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveOrganizationMembersInput;
};
export type organizationMembers_removeOrganizationMembersInputMutation$data = {
  readonly removeOrganizationMembersInput: {
    readonly members: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMembers_removeOrganizationMembersInputMutation = {
  response: organizationMembers_removeOrganizationMembersInputMutation$data;
  variables: organizationMembers_removeOrganizationMembersInputMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationMembers_removeOrganizationMembersInputMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationMembersInput",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
            "selections": [
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationMembers_removeOrganizationMembersInputMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationMembersInput",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
            "selections": [
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "e5240821ecf191ea600ad2beaff49996",
    "id": null,
    "metadata": {},
    "name": "organizationMembers_removeOrganizationMembersInputMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembers_removeOrganizationMembersInputMutation(\n  $input: RemoveOrganizationMembersInput!\n) {\n  removeOrganizationMembersInput(input: $input) {\n    members {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a4be97dca8bc154fcc6223a6f31708f3";

export default node;
