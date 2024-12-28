/**
 * @generated SignedSource<<1a1b740febdc6a7c423033e74fbef524>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveTeamMemberInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type teamMemberCard_removeTeamMemberMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveTeamMemberInput;
};
export type teamMemberCard_removeTeamMemberMutation$data = {
  readonly removeTeamMember: {
    readonly teamMember: {
      readonly id: string;
    };
  } | null | undefined;
};
export type teamMemberCard_removeTeamMemberMutation = {
  response: teamMemberCard_removeTeamMemberMutation$data;
  variables: teamMemberCard_removeTeamMemberMutation$variables;
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
    "name": "teamMemberCard_removeTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "removeTeamMember",
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
    "name": "teamMemberCard_removeTeamMemberMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMemberPayload",
        "kind": "LinkedField",
        "name": "removeTeamMember",
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
    "cacheID": "679c4ab88dfa06b145441af1e7ecd718",
    "id": null,
    "metadata": {},
    "name": "teamMemberCard_removeTeamMemberMutation",
    "operationKind": "mutation",
    "text": "mutation teamMemberCard_removeTeamMemberMutation(\n  $input: RemoveTeamMemberInput!\n) {\n  removeTeamMember(input: $input) {\n    teamMember {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "eb35c2e76d57edad7b744d491738fd0d";

export default node;
