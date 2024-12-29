/**
 * @generated SignedSource<<e1d3c943dba364a319361057e831919e>>
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
export type organizationMembers_removeOrganizationMembersMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveOrganizationMembersInput;
};
export type organizationMembers_removeOrganizationMembersMutation$data = {
  readonly removeOrganizationMembers: {
    readonly members: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMembers_removeOrganizationMembersMutation = {
  response: organizationMembers_removeOrganizationMembersMutation$data;
  variables: organizationMembers_removeOrganizationMembersMutation$variables;
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
    "name": "organizationMembers_removeOrganizationMembersMutation",
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
    "name": "organizationMembers_removeOrganizationMembersMutation",
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
    "cacheID": "7eb34bb98be471cef79397d13c287836",
    "id": null,
    "metadata": {},
    "name": "organizationMembers_removeOrganizationMembersMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMembers_removeOrganizationMembersMutation(\n  $input: RemoveOrganizationMembersInput!\n) {\n  removeOrganizationMembers(input: $input) {\n    members {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "475d3e6e7f00b616d1e2d91777184491";

export default node;
