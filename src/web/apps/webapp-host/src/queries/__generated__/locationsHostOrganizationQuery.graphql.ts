/**
 * @generated SignedSource<<29c5dcde1e1491061569d71fd9f5e5cd>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type locationsHostOrganizationQuery$variables = Record<PropertyKey, never>;
export type locationsHostOrganizationQuery$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly uniqueId: string;
  }>;
};
export type locationsHostOrganizationQuery = {
  response: locationsHostOrganizationQuery$data;
  variables: locationsHostOrganizationQuery$variables;
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
    "name": "locationsHostOrganizationQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "locationsHostOrganizationQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "faeacba12e73cec78b76383477a5e2c3",
    "id": null,
    "metadata": {},
    "name": "locationsHostOrganizationQuery",
    "operationKind": "query",
    "text": "query locationsHostOrganizationQuery {\n  myOrganizations(types: [HOST]) {\n    uniqueId\n  }\n}\n"
  }
};
})();

(node as any).hash = "2b062155b55059d10aac8e85df2b5358";

export default node;
