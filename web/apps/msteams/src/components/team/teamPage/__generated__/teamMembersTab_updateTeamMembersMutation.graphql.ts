/**
 * @generated SignedSource<<6938fde8e141f62b831706a84fe3abeb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateTeamMembersInput = {
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  id: string;
  organizationMemberIds: ReadonlyArray<string>;
};
export type teamMembersTab_updateTeamMembersMutation$variables = {
  input: UpdateTeamMembersInput;
};
export type teamMembersTab_updateTeamMembersMutation$data = {
  readonly updateTeamMembers: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly members: ReadonlyArray<{
        readonly customer: {
          readonly uniqueId: string;
        };
        readonly organizationMember: {
          readonly uniqueId: string;
        } | null | undefined;
      }>;
      readonly name: string;
      readonly organization: {
        readonly name: string;
      } | null | undefined;
    };
  } | null | undefined;
};
export type teamMembersTab_updateTeamMembersMutation$rawResponse = {
  readonly updateTeamMembers: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly members: ReadonlyArray<{
        readonly customer: {
          readonly uniqueId: string;
        };
        readonly id: string;
        readonly organizationMember: {
          readonly uniqueId: string;
        } | null | undefined;
      }>;
      readonly name: string;
      readonly organization: {
        readonly name: string;
      } | null | undefined;
    };
  } | null | undefined;
};
export type teamMembersTab_updateTeamMembersMutation = {
  rawResponse: teamMembersTab_updateTeamMembersMutation$rawResponse;
  response: teamMembersTab_updateTeamMembersMutation$data;
  variables: teamMembersTab_updateTeamMembersMutation$variables;
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
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "about",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamOrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    (v3/*: any*/)
  ],
  "storageKey": null
},
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
],
v7 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": (v6/*: any*/),
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamOrganizationMemberDetails",
  "kind": "LinkedField",
  "name": "organizationMember",
  "plural": false,
  "selections": (v6/*: any*/),
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamMembersTab_updateTeamMembersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamPayload",
        "kind": "LinkedField",
        "name": "updateTeamMembers",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              (v4/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v7/*: any*/),
                  (v8/*: any*/)
                ],
                "storageKey": null
              }
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
    "name": "teamMembersTab_updateTeamMembersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamPayload",
        "kind": "LinkedField",
        "name": "updateTeamMembers",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v3/*: any*/),
              (v4/*: any*/),
              (v5/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v7/*: any*/),
                  (v8/*: any*/),
                  (v2/*: any*/)
                ],
                "storageKey": null
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
    "cacheID": "9057733ea805786fb68dc8bdcd6ece7d",
    "id": null,
    "metadata": {},
    "name": "teamMembersTab_updateTeamMembersMutation",
    "operationKind": "mutation",
    "text": "mutation teamMembersTab_updateTeamMembersMutation(\n  $input: UpdateTeamMembersInput!\n) {\n  updateTeamMembers(input: $input) {\n    team {\n      id\n      name\n      about\n      organization {\n        name\n      }\n      members {\n        customer {\n          uniqueId\n        }\n        organizationMember {\n          uniqueId\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3d875c05c633c0b9c56dd7c58b45ef9d";

export default node;
