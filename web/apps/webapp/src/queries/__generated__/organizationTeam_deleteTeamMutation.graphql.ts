/**
 * @generated SignedSource<<3c31d59136c532e6f7461b56ac6bbc79>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type organizationTeam_deleteTeamMutation$variables = {
  input: DeleteTeamInput;
};
export type organizationTeam_deleteTeamMutation$data = {
  readonly deleteTeam: {
    readonly team: {
      readonly id: string;
    };
  };
};
export type organizationTeam_deleteTeamMutation = {
  response: organizationTeam_deleteTeamMutation$data;
  variables: organizationTeam_deleteTeamMutation$variables;
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
    "name": "deleteTeam",
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
    "name": "organizationTeam_deleteTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeam_deleteTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "f9cd8ef67fa3d8c0da7f80477273c7bc",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_deleteTeamMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_deleteTeamMutation(\n  $input: DeleteTeamInput!\n) {\n  deleteTeam(input: $input) {\n    team {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cad5e1d9e4eb4435588a254154b0f6ab";

export default node;
