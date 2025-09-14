/**
 * @generated SignedSource<<6847f24520569123c2185fa2cfe2ac39>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type OrganizationMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type PersonalInformationVisibility = "REDACTED" | "VISIBLE" | "%future added value";
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
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly personalInformationVisibility: {
          readonly name: string;
          readonly type: PersonalInformationVisibility;
        };
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      };
      readonly id: string;
      readonly role: OrganizationMemberRole | null | undefined;
      readonly status: OrganizationMemberStatus;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
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
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v2/*: any*/),
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
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PersonalInformationVisibilityDetails",
                "kind": "LinkedField",
                "name": "personalInformationVisibility",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "type",
                    "storageKey": null
                  },
                  (v2/*: any*/)
                ],
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
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "56be94db710730ec69a918b9288cb92d",
    "id": null,
    "metadata": {},
    "name": "organizationUser_changeOrganizationUsersStatusMutation",
    "operationKind": "mutation",
    "text": "mutation organizationUser_changeOrganizationUsersStatusMutation(\n  $input: ChangeOrganizationMembersStatusInput!\n) {\n  changeOrganizationMembersStatus(input: $input) {\n    members {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n        personalInformationVisibility {\n          type\n          name\n        }\n      }\n      status\n      role\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ffe52fae3c4d15da4b97e6e674f560f1";

export default node;
