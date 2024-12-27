/**
 * @generated SignedSource<<62740aa19ebcd8cc99fc3d4b4ddd84b4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationTeam_rootQuery$variables = {
  teamId: string;
};
export type organizationTeam_rootQuery$data = {
  readonly team: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type organizationTeam_rootQuery = {
  response: organizationTeam_rootQuery$data;
  variables: organizationTeam_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "teamId"
      }
    ],
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
    "name": "organizationTeam_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationTeam_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "5630d5f0774e26ef8806cb9589d285bc",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_rootQuery",
    "operationKind": "query",
    "text": "query organizationTeam_rootQuery(\n  $teamId: String!\n) {\n  team(id: $teamId) {\n    id\n    name\n    about\n  }\n}\n"
  }
};
})();

(node as any).hash = "9cc1e6e22adb19aec686db6f5b12dc9d";

export default node;
