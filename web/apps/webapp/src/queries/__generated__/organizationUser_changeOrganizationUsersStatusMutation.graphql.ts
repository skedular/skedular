/**
 * @generated SignedSource<<d9f8f628ab844ddf57b496f293b831ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
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
      readonly customer: {
        readonly email: string | null | undefined;
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly id: string;
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
    }>;
  } | null | undefined;
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
            "concreteType": "Organization_CustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "name",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "givenName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "middleName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "familyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "photoUrl",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "phoneNumber",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "status",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "role",
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
    "cacheID": "3afaba6d43e1eafb35dbe31587666584",
    "id": null,
    "metadata": {},
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUser_changeOrganizationUsersStatusMutation(\n  $input: ChangeOrganizationMembersStatusInput!\n) {\n  changeOrganizationMembersStatus(input: $input) {\n    members {\n      id\n      customer {\n        uniqueId\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9fb1a7e1984af370a20b85eaf34bac86";

export default node;
