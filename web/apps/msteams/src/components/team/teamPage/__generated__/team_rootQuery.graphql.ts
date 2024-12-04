/**
 * @generated SignedSource<<c32a7f95ee77476f52ee5bdc664343a7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type team_rootQuery$variables = {
  organizationId: string;
  teamId: string;
};
export type team_rootQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly team: {
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
};
export type team_rootQuery = {
  response: team_rootQuery$data;
  variables: team_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "organizationId"
      }
    ],
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "organization",
    "plural": false,
    "selections": [
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "storageKey": null
  },
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
      (v1/*: any*/),
      (v2/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueId",
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
    "name": "team_rootQuery",
    "selections": (v3/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "team_rootQuery",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "8cc1de8e5852aea97ba18b3d6ced169c",
    "id": null,
    "metadata": {},
    "name": "team_rootQuery",
    "operationKind": "query",
    "text": "query team_rootQuery(\n  $organizationId: String!\n  $teamId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  team(id: $teamId) {\n    id\n    name\n    organization {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "8f4413b385a9a76b242dc5b08df9d6ae";

export default node;
