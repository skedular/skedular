/**
 * @generated SignedSource<<dabb784f18c6673faeaf47afc67d785e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteTeamMemberInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type teamMemberCard_deleteTeamMemberMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteTeamMemberInput;
};
export type teamMemberCard_deleteTeamMemberMutation$data = {
  readonly deleteTeamMember: {
    readonly teamMember: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamMemberCard_deleteTeamMemberMutation = {
  response: teamMemberCard_deleteTeamMemberMutation$data;
  variables: teamMemberCard_deleteTeamMemberMutation$variables;
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
    "name": "teamMemberCard_deleteTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "deleteTeamMember",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "teamMember",
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
    "name": "teamMemberCard_deleteTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "deleteTeamMember",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "teamMember",
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
    "cacheID": "265fa3c40b1cde0c9fdd50fa0df145f1",
    "id": null,
    "metadata": {},
    "name": "teamMemberCard_deleteTeamMemberMutation",
    "operationKind": "mutation",
    "text": "mutation teamMemberCard_deleteTeamMemberMutation(\n  $input: DeleteTeamMemberInput!\n) {\n  deleteTeamMember(input: $input) {\n    teamMember {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "322f52f3bc1170980dc21a61e0d521ba";

export default node;
