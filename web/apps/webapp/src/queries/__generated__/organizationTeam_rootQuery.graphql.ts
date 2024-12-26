/**
 * @generated SignedSource<<f49397ba198e992f45fc75ff67efaaa8>>
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
    "cacheID": "67cb503fcdb8cbc38639bf26810d2e92",
    "id": null,
    "metadata": {},
    "name": "organizationTeam_rootQuery",
    "operationKind": "query",
    "text": "query organizationTeam_rootQuery(\n  $teamId: String!\n) {\n  team(id: $teamId) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "2bc8a634db81a49eb719121c71698f97";

export default node;
