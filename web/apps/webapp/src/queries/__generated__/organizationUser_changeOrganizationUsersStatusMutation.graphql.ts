/**
 * @generated SignedSource<<3586807f2b1d9fa1d7af925230c3da44>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type ChangeOrganizationMembersStatusInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
  status: OrganizationMemberStatus;
};
export type organizationUser_changeOrganizationUsersStatusMutation$variables = {
  input: ChangeOrganizationMembersStatusInput;
};
export type organizationUser_changeOrganizationUsersStatusMutation$data = {
  readonly changeOrganizationMembersStatus: {
    readonly members: ReadonlyArray<{
      readonly id: string;
      readonly status: {
        readonly name: string;
        readonly type: OrganizationMemberStatus;
      };
    }>;
  };
};
export type organizationUser_changeOrganizationUsersStatusMutation = {
  response: organizationUser_changeOrganizationUsersStatusMutation$data;
  variables: organizationUser_changeOrganizationUsersStatusMutation$variables;
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
    "concreteType": "OrganizationMembersDetailsPayload",
    "kind": "LinkedField",
    "name": "changeOrganizationMembersStatus",
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberStatusDetails",
            "kind": "LinkedField",
            "name": "status",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
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
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "332e7a4cf5964f46f9b963557bcd327b",
    "id": null,
    "metadata": {},
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUser_changeOrganizationUsersStatusMutation(\n  $input: ChangeOrganizationMembersStatusInput!\n) {\n  changeOrganizationMembersStatus(input: $input) {\n    members {\n      id\n      status {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "156427a798869fed669fdb34d4fb34cf";

export default node;
