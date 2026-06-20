/**
 * @generated SignedSource<<8d5c8446da709d56782365b911105499>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type hostOperationsQuery$variables = Record<PropertyKey, never>;
export type hostOperationsQuery$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly customDomain: string | null | undefined;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly uniqueId: string;
    readonly website: string | null | undefined;
  }>;
};
export type hostOperationsQuery = {
  response: hostOperationsQuery$data;
  variables: hostOperationsQuery$variables;
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
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "customDomain",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "logoUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "website",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "contactEmail",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "contactPhone",
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
    "name": "hostOperationsQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "hostOperationsQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "03c13523342cb5c1e3e241d27adbc51d",
    "id": null,
    "metadata": {},
    "name": "hostOperationsQuery",
    "operationKind": "query",
    "text": "query hostOperationsQuery {\n  myOrganizations(types: [HOST]) {\n    uniqueId\n    name\n    customDomain\n    logoUrl\n    website\n    contactEmail\n    contactPhone\n  }\n}\n"
  }
};
})();

(node as any).hash = "03a7af7932d5875e7b521ceac1197bd0";

export default node;
