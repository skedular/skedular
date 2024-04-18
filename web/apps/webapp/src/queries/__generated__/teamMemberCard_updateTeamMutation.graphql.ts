/**
 * @generated SignedSource<<e7d6aa41f11b100e9a5050c27d1d828e>>
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
export type teamMemberCard_updateTeamMutation$variables = {
  input: UpdateTeamInput;
};
export type teamMemberCard_updateTeamMutation$data = {
  readonly updateTeam: {
    readonly team: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamMemberCard_updateTeamMutation$rawResponse = {
  readonly updateTeam: {
    readonly team: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamMemberCard_updateTeamMutation = {
  rawResponse: teamMemberCard_updateTeamMutation$rawResponse;
  response: teamMemberCard_updateTeamMutation$data;
  variables: teamMemberCard_updateTeamMutation$variables;
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
    "name": "teamMemberCard_updateTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamMemberCard_updateTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "52877fcdac1752251ab36db14dba5ee6",
    "id": null,
    "metadata": {},
    "name": "teamMemberCard_updateTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamMemberCard_updateTeamMutation(\n  $input: UpdateTeamInput!\n) {\n  updateTeam(input: $input) {\n    team {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ee7fa9346838b0b11edda7b7fe5858ed";

export default node;
