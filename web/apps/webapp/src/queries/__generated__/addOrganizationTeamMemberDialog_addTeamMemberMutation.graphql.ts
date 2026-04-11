/**
 * @generated SignedSource<<dcf7f123112829bffef96b138359c71e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type TeamMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
export type TeamMemberStatus = "ACTIVE" | "INACTIVE" | "%future added value";
export type AddTeamMemberInput = {
  clientMutationId?: string | null | undefined;
  customerId?: string | null | undefined;
  id: string;
  organizationMemberId?: string | null | undefined;
};
export type addOrganizationTeamMemberDialog_addTeamMemberMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddTeamMemberInput;
};
export type addOrganizationTeamMemberDialog_addTeamMemberMutation$data = {
  readonly addTeamMember: {
    readonly teamMember: {
      readonly customer: {
        readonly email: string | null | undefined;
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly phoneNumber: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      };
      readonly id: string;
      readonly role: {
        readonly name: string;
        readonly type: TeamMemberRole;
      };
      readonly status: {
        readonly name: string;
        readonly type: TeamMemberStatus;
      };
    };
  };
};
export type addOrganizationTeamMemberDialog_addTeamMemberMutation = {
  response: addOrganizationTeamMemberDialog_addTeamMemberMutation$data;
  variables: addOrganizationTeamMemberDialog_addTeamMemberMutation$variables;
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
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*: any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamMemberDetails",
  "kind": "LinkedField",
  "name": "teamMember",
  "plural": false,
  "selections": [
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "customer",
      "plural": false,
      "selections": [
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "email",
          "storageKey": null
        },
        (v3/*: any*/),
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
      "concreteType": "TeamMemberStatusDetails",
      "kind": "LinkedField",
      "name": "status",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamMemberRoleDetails",
      "kind": "LinkedField",
      "name": "role",
      "plural": false,
      "selections": (v4/*: any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addOrganizationTeamMemberDialog_addTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "addTeamMember",
        "plural": false,
        "selections": [
          (v5/*: any*/)
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
    "name": "addOrganizationTeamMemberDialog_addTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "addTeamMember",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "teamMember",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "TeamMemberDetails"
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "42e5dd3f16db5f018aa4153f5b07c163",
    "id": null,
    "metadata": {},
    "name": "addOrganizationTeamMemberDialog_addTeamMemberMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganizationTeamMemberDialog_addTeamMemberMutation(\n  $input: AddTeamMemberInput!\n) {\n  addTeamMember(input: $input) {\n    teamMember {\n      id\n      customer {\n        id\n        email\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n        phoneNumber\n      }\n      status {\n        type\n        name\n      }\n      role {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ac54d72689cbcc2657667e6ec59a6566";

export default node;
