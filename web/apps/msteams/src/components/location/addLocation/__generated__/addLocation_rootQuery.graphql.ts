/**
 * @generated SignedSource<<90da998a9c8553c1b866dfd23109a7c7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type addLocation_rootQuery$variables = {
  organizationExists: boolean;
  organizationId: string;
};
export type addLocation_rootQuery$data = {
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type addLocation_rootQuery = {
  response: addLocation_rootQuery$data;
  variables: addLocation_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v2 = [
  {
    "condition": "organizationExists",
    "kind": "Condition",
    "passingValue": true,
    "selections": [
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
    ]
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addLocation_rootQuery",
    "selections": (v2/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "addLocation_rootQuery",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "594e70d984adc75e2eb6338e8e56c36f",
    "id": null,
    "metadata": {},
    "name": "addLocation_rootQuery",
    "operationKind": "query",
    "text": "query addLocation_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n) {\n  organization(id: $organizationId) @include(if: $organizationExists) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "ef79f0dd3ab1fa498decbdbb183b2cc6";

export default node;
