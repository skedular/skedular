/**
 * @generated SignedSource<<612b2e728cf90ffd8fcfdb166e975958>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type createUnifiedHostLocationOrganizationQuery$variables = Record<PropertyKey, never>;
export type createUnifiedHostLocationOrganizationQuery$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly uniqueId: string;
  }>;
};
export type createUnifiedHostLocationOrganizationQuery = {
  response: createUnifiedHostLocationOrganizationQuery$data;
  variables: createUnifiedHostLocationOrganizationQuery$variables;
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
    "name": "createUnifiedHostLocationOrganizationQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "createUnifiedHostLocationOrganizationQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "0967ba55dfce06c99737d7f785c2d716",
    "id": null,
    "metadata": {},
    "name": "createUnifiedHostLocationOrganizationQuery",
    "operationKind": "query",
    "text": "query createUnifiedHostLocationOrganizationQuery {\n  myOrganizations(types: [HOST]) {\n    uniqueId\n  }\n}\n"
  }
};
})();

(node as any).hash = "e06ee0274df6fd1d3adf468aa665cfae";

export default node;
