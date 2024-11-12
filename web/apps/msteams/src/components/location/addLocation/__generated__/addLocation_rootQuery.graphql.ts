/**
 * @generated SignedSource<<dfd4ccb40f0105fd7e36c3218898ac7b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type addLocation_rootQuery$variables = {
  organizationId: string;
};
export type addLocation_rootQuery$data = {
  readonly organization: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type addLocation_rootQuery = {
  response: addLocation_rootQuery$data;
  variables: addLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = [
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
    "name": "addLocation_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addLocation_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "132c4b827d2713c43977fd18956f14f7",
    "id": null,
    "metadata": {},
    "name": "addLocation_rootQuery",
    "operationKind": "query",
    "text": "query addLocation_rootQuery(\n  $organizationId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "cb5ead54c88153835e25925bedca3bcc";

export default node;
