/**
 * @generated SignedSource<<6695fb37be81ef47f88a4b22fc788d34>>
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
  id: string;
  name: string;
  organizationId?: string | null | undefined;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "timezone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": [
              (v1/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamLocationDetails",
            "kind": "LinkedField",
            "name": "primaryLocation",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              (v1/*: any*/)
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
    "name": "teamAboutTab_updateTeamMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamAboutTab_updateTeamMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "bc2390b7f8030ce6d8b1445cbbf4f347",
    "id": null,
    "metadata": {},
    "name": "teamAboutTab_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamAboutTab_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      organization {\n        name\n      }\n      primaryLocation {\n        uniqueId\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "de04e3fbab9d7bf632f455f1fcc4ee8f";

export default node;
