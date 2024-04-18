/**
 * @generated SignedSource<<fae8a64583809e55043b2ac42a537652>>
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
export type teamPeopleTab_updateTeamMutation$variables = {
  input: UpdateTeamInput;
};
export type teamPeopleTab_updateTeamMutation$data = {
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
    };
  } | null | undefined;
};
export type teamPeopleTab_updateTeamMutation$rawResponse = {
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
    };
  } | null | undefined;
};
export type teamPeopleTab_updateTeamMutation = {
  rawResponse: teamPeopleTab_updateTeamMutation$rawResponse;
  response: teamPeopleTab_updateTeamMutation$data;
  variables: teamPeopleTab_updateTeamMutation$variables;
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
    "name": "teamPeopleTab_updateTeamMutation",
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
    "name": "teamPeopleTab_updateTeamMutation",
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
    "cacheID": "3427c1dca1c5c86b309ec8e7974f7f3b",
    "id": null,
    "metadata": {},
    "name": "teamPeopleTab_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleTab_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      organization {\n        name\n      }\n      members {\n        customer {\n          uniqueId\n        }\n        organizationMember {\n          uniqueId\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "899514d1c94f212822fcc3c722b69cf4";

export default node;
