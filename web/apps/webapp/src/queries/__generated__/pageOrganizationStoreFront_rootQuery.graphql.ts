/**
 * @generated SignedSource<<89776f0e12e6b413a2fe88e1ab050636>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageOrganizationStoreFront_rootQuery$variables = Record<PropertyKey, never>;
export type pageOrganizationStoreFront_rootQuery$data = {
  readonly bookingVersion: {
    readonly major: number;
  };
};
export type pageOrganizationStoreFront_rootQuery = {
  response: pageOrganizationStoreFront_rootQuery$data;
  variables: pageOrganizationStoreFront_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "Version",
    "kind": "LinkedField",
    "name": "bookingVersion",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "major",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationStoreFront_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageOrganizationStoreFront_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "52f995b44b159bcb362aca07fd9d3a34",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationStoreFront_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationStoreFront_rootQuery {\n  bookingVersion {\n    major\n  }\n}\n"
  }
};
})();

(node as any).hash = "81351f08090cb57d3292847d6d09b2ae";

export default node;
