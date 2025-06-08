/**
 * @generated SignedSource<<a790e682b500124b3b5b31714cd831b4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type AddTeamInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  id?: string | null | undefined;
  name: string;
  organizationId: string;
  organizationMemberIds: ReadonlyArray<string>;
  primaryFeatureImageUrl?: string | null | undefined;
  primaryLocationId?: string | null | undefined;
  timezone?: string | null | undefined;
};
export type addTeam_addTeamMutation$variables = {
  input: AddTeamInput;
};
export type addTeam_addTeamMutation$data = {
  readonly addTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly primaryFeatureImageUrl: string | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addTeam_addTeamMutation$rawResponse = {
  readonly addTeam: {
    readonly team: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly name: string;
      readonly primaryFeatureImageUrl: string | null | undefined;
      readonly timezone: string | null | undefined;
    };
  } | null | undefined;
};
export type addTeam_addTeamMutation = {
  rawResponse: addTeam_addTeamMutation$rawResponse;
  response: addTeam_addTeamMutation$data;
  variables: addTeam_addTeamMutation$variables;
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
    "concreteType": "TeamPayload",
    "kind": "LinkedField",
    "name": "addTeam",
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
            "kind": "ScalarField",
            "name": "primaryFeatureImageUrl",
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
    "name": "addTeam_addTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addTeam_addTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "459b593fd62c48cd92264c7c60e0b20a",
    "id": null,
    "metadata": {},
    "name": "addTeam_addTeamMutation",
    "operationKind": "mutation",
    "text": "mutation addTeam_addTeamMutation(\n  $input: AddTeamInput!\n) {\n  addTeam(input: $input) {\n    team {\n      id\n      name\n      about\n      timezone\n      primaryFeatureImageUrl\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "94936ae8eeb201f075a89ca7ed1b5937";

export default node;
