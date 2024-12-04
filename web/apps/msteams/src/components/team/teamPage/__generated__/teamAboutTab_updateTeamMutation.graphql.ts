/**
 * @generated SignedSource<<7ea2a206a571960d4e811d8543d57ff7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateTeamInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  id: string;
  name: string;
  organizationId?: string | null | undefined;
  organizationMemberIds: ReadonlyArray<string>;
  primaryLocationId?: string | null | undefined;
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
        };
        readonly organizationMember: {
          readonly uniqueId: string;
        } | null | undefined;
      }>;
      readonly name: string;
      readonly organization: {
        readonly name: string;
      } | null | undefined;
      readonly primaryLocation: {
        readonly name: string;
        readonly uniqueId: string;
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
      readonly primaryLocation: {
        readonly name: string;
        readonly uniqueId: string;
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
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamLocationDetails",
  "kind": "LinkedField",
  "name": "primaryLocation",
  "plural": false,
  "selections": [
    (v7/*: any*/),
    (v3/*: any*/)
  ],
  "storageKey": null
},
v9 = [
  (v7/*: any*/)
],
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamCustomerDetails",
  "kind": "LinkedField",
  "name": "customer",
  "plural": false,
  "selections": (v9/*: any*/),
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "concreteType": "TeamOrganizationMemberDetails",
  "kind": "LinkedField",
  "name": "organizationMember",
  "plural": false,
  "selections": (v9/*: any*/),
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
              (v8/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v10/*: any*/),
                  (v11/*: any*/)
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
              (v8/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberDetails",
                "kind": "LinkedField",
                "name": "members",
                "plural": true,
                "selections": [
                  (v10/*: any*/),
                  (v11/*: any*/),
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
    "cacheID": "3b23b279ae4ba90c083929030535da6f",
    "id": null,
    "metadata": {},
    "name": "teamAboutTab_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamAboutTab_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      organization {\n        name\n      }\n      primaryLocation {\n        uniqueId\n        name\n      }\n      members {\n        customer {\n          uniqueId\n        }\n        organizationMember {\n          uniqueId\n        }\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "92dc8949524470f00dbec84a32a0ddaa";

export default node;
