/**
 * @generated SignedSource<<f68f51cbec4a6c7b59eec4bfccadf0b2>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type dashboardHostOrganizationQuery$variables = Record<PropertyKey, never>;
export type dashboardHostOrganizationQuery$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly name: string;
    readonly uniqueId: string;
  }>;
};
export type dashboardHostOrganizationQuery = {
  response: dashboardHostOrganizationQuery$data;
  variables: dashboardHostOrganizationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Literal",
        "name": "types",
        "value": [
          "HOST"
        ]
      }
    ],
    "concreteType": "MyOrganizationDetails",
    "kind": "LinkedField",
    "name": "myOrganizations",
    "plural": true,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "uniqueId",
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
    "storageKey": "myOrganizations(types:[\"HOST\"])"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "dashboardHostOrganizationQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "dashboardHostOrganizationQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "d46a6bd943a253586735c1bdf61671b6",
    "id": null,
    "metadata": {},
    "name": "dashboardHostOrganizationQuery",
    "operationKind": "query",
    "text": "query dashboardHostOrganizationQuery {\n  myOrganizations(types: [HOST]) {\n    uniqueId\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "92e44d4e11b7863ad2d9617f7d4ca574";

export default node;
