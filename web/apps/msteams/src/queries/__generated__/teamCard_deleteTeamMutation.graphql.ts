/**
 * @generated SignedSource<<350dfff77c4439ebc5529b8d12ab4b51>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type DeleteTeamInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type teamCard_deleteTeamMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteTeamInput;
};
export type teamCard_deleteTeamMutation$data = {
  readonly deleteTeam: {
    readonly team: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamCard_deleteTeamMutation = {
  response: teamCard_deleteTeamMutation$data;
  variables: teamCard_deleteTeamMutation$variables;
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
    "name": "teamCard_deleteTeamMutation",
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
    "name": "teamCard_deleteTeamMutation",
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
    "cacheID": "32fb7372578b46a15435fe9f32987bda",
    "id": null,
    "metadata": {},
    "name": "teamCard_deleteTeamMutation",
    "operationKind": "mutation",
    "text": "mutation teamCard_deleteTeamMutation(\n  $input: DeleteTeamInput!\n) {\n  deleteTeam(input: $input) {\n    team {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0068804c1651e467076babe1bc624b53";

export default node;
