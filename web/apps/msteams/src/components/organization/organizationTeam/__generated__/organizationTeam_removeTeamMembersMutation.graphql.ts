/**
 * @generated SignedSource<<1b6177d037d87047ecc57c1f1c18a9f8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type RemoveTeamMembersInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type organizationTeam_removeTeamMembersMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: RemoveTeamMembersInput;
};
export type organizationTeam_removeTeamMembersMutation$data = {
  readonly removeTeamMembers: {
    readonly members: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationTeam_removeTeamMembersMutation = {
  response: organizationTeam_removeTeamMembersMutation$data;
  variables: organizationTeam_removeTeamMembersMutation$variables;
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
    "name": "organizationTeam_removeTeamMembersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeTeamMembers",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
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
    "name": "organizationTeam_removeTeamMembersMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "TeamMembersDetailsPayload",
        "kind": "LinkedField",
        "name": "removeTeamMembers",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamMemberDetails",
            "kind": "LinkedField",
            "name": "members",
            "plural": true,
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
    "cacheID": "2270b5bb2b77ea1418fec54125dd1679",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_removeTeamMembersMutation",
    "operationKind": "mutation",
    "text": "mutation organizationTeam_removeTeamMembersMutation(\n  $input: RemoveTeamMembersInput!\n) {\n  removeTeamMembers(input: $input) {\n    members {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "40e8fd85e713dafb311cb03fe23700c8";

export default node;
