/**
 * @generated SignedSource<<37743f072eb37e5cc835a03f6fbbf215>>
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
export type organizationUser_removeOrganizationUsersMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveOrganizationMembersInput;
};
export type organizationUser_removeOrganizationUsersMutation$data = {
  readonly removeOrganizationMembers: {
    readonly members: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationUser_removeOrganizationUsersMutation = {
  response: organizationUser_removeOrganizationUsersMutation$data;
  variables: organizationUser_removeOrganizationUsersMutation$variables;
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
    "name": "organizationUser_removeOrganizationUsersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationMembers",
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
    "name": "organizationUser_removeOrganizationUsersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeOrganizationMembers",
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
    "cacheID": "9954f8563a3e14ebd615074a9d2e7a58",
    "id": null,
    "metadata": {},
    "name": "organizationUser_removeOrganizationUsersMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUser_removeOrganizationUsersMutation(\n  $input: RemoveOrganizationMembersInput!\n) {\n  removeOrganizationMembers(input: $input) {\n    members {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ddfa1de4b3107b340f94ecb0e96dbe5e";

export default node;
