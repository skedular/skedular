/**
 * @generated SignedSource<<15412b429b83bc577c044fa02853c7ce>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveOrganizationMembersInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationUsers_removeOrganizationUsersMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveOrganizationMembersInput;
};
export type organizationUsers_removeOrganizationUsersMutation$data = {
  readonly removeOrganizationMembers: {
    readonly members: ReadonlyArray<{
      readonly id: string;
    }>;
  };
};
export type organizationUsers_removeOrganizationUsersMutation = {
  response: organizationUsers_removeOrganizationUsersMutation$data;
  variables: organizationUsers_removeOrganizationUsersMutation$variables;
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationUsers_removeOrganizationUsersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
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
              (v2/*:: as any*/)
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
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "organizationUsers_removeOrganizationUsersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
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
              (v2/*:: as any*/),
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
    "cacheID": "ff00ab17ce446aaac702aca6b2f74479",
    "id": null,
    "metadata": {},
    "name": "organizationUsers_removeOrganizationUsersMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUsers_removeOrganizationUsersMutation(\n  $input: RemoveOrganizationMembersInput!\n) {\n  removeOrganizationMembers(input: $input) {\n    members {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "64d83e56810b846f3ab6bc95c78f3173";

export default node;
