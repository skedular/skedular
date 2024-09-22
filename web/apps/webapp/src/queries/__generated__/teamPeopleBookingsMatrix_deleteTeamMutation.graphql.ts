/**
 * @generated SignedSource<<e6fa1df2e58d2b64a1484af82f07dd14>>
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
export type teamPeopleBookingsMatrix_deleteTeamMutation$variables = {
  input: DeleteTeamInput;
};
export type teamPeopleBookingsMatrix_deleteTeamMutation$data = {
  readonly deleteTeam: {
    readonly team: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamPeopleBookingsMatrix_deleteTeamMutation = {
  response: teamPeopleBookingsMatrix_deleteTeamMutation$data;
  variables: teamPeopleBookingsMatrix_deleteTeamMutation$variables;
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
    "name": "teamPeopleBookingsMatrix_deleteTeamMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "teamPeopleBookingsMatrix_deleteTeamMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "2b6de521d3b57c0747aee32350ae6a4f",
    "id": null,
    "metadata": {},
    "name": "teamPeopleBookingsMatrix_deleteTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleBookingsMatrix_deleteTeamMutation(\n  $input: DeleteTeamInput!\n) {\n  deleteTeam(input: $input) {\n    team {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ebacd0d1ce7ab6081373bee469b20b1f";

export default node;
