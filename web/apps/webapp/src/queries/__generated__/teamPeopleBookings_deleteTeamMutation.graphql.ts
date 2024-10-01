/**
 * @generated SignedSource<<7ed52d6e379f50ca9a52f5265da3046f>>
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
export type teamPeopleBookings_deleteTeamMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteTeamInput;
};
export type teamPeopleBookings_deleteTeamMutation$data = {
  readonly deleteTeam: {
    readonly team: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamPeopleBookings_deleteTeamMutation = {
  response: teamPeopleBookings_deleteTeamMutation$data;
  variables: teamPeopleBookings_deleteTeamMutation$variables;
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "teamPeopleBookings_deleteTeamMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/)
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
    "name": "teamPeopleBookings_deleteTeamMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
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
    "cacheID": "08d6acb91755151cd350e696847a6e3b",
    "id": null,
    "metadata": {},
    "name": "teamPeopleBookings_deleteTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamPeopleBookings_deleteTeamMutation(\n  $input: DeleteTeamInput!\n) {\n  deleteTeam(input: $input) {\n    team {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d3f35709fd5b74fd3917a15eb1de6060";

export default node;
