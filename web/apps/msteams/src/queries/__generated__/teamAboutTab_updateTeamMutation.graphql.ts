/**
 * @generated SignedSource<<ad7750e02a9baf3c424d376e2ae25df9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type UpdateTeamInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  id: string;
  name: string;
  organizationId?: string | null | undefined;
  organizationMemberIds: ReadonlyArray<string>;
  timezone?: string | null | undefined;
};
export type teamAboutTab_updateTeamMutation$variables = {
  input: UpdateTeamInput;
};
export type teamAboutTab_updateTeamMutation$data = {
  readonly updateTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly members: ReadonlyArray<{
        readonly customer: {
          readonly uniqueId: string;
        } | null | undefined;
        readonly organizationMember: {
          readonly uniqueId: string;
        } | null | undefined;
      }>;
      readonly name: string;
      readonly organization: {
        readonly name: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type teamAboutTab_updateTeamMutation$rawResponse = {
  readonly updateTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly members: ReadonlyArray<{
        readonly customer: {
          readonly uniqueId: string;
        } | null | undefined;
        readonly id: string;
        readonly organizationMember: {
          readonly uniqueId: string;
        } | null | undefined;
      }>;
      readonly name: string;
      readonly organization: {
        readonly name: string;
      } | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type teamAboutTab_updateTeamMutation = {
  rawResponse: teamAboutTab_updateTeamMutation$rawResponse;
  response: teamAboutTab_updateTeamMutation$data;
  variables: teamAboutTab_updateTeamMutation$variables;
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
  "kind": "ScalarField",
  "name": "timezone",
  "storageKey": null
},
v6 = {
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
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": (v7/*: any*/),
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamOrganizationMemberDetails",
  "kind": "LinkedField",
  "name": "organizationMember",
  "plural": false,
  "selections": (v7/*: any*/),
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamAboutTab_updateTeamMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamPayload",
        "kind": "LinkedField",
        "name": "updateTeam",
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
              (v6/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v8/*: any*/),
                  (v9/*: any*/)
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
    "name": "teamAboutTab_updateTeamMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamPayload",
        "kind": "LinkedField",
        "name": "updateTeam",
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
              (v6/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v8/*: any*/),
                  (v9/*: any*/),
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
    "cacheID": "a6c5344e80df5ab03654f3f8a2b26a88",
    "id": null,
    "metadata": {},
    "name": "teamAboutTab_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamAboutTab_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      organization {\n        name\n      }\n      members {\n        customer {\n          uniqueId\n        }\n        organizationMember {\n          uniqueId\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "48ae6e4fd051150a9836bbbf7619fa95";

export default node;
