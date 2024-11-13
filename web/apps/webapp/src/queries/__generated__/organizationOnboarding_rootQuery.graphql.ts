/**
 * @generated SignedSource<<12160a8134699deb2ce9f715d4694a06>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organizationOnboarding_rootQuery$variables = Record<PropertyKey, never>;
export type organizationOnboarding_rootQuery$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
};
export type organizationOnboarding_rootQuery = {
  response: organizationOnboarding_rootQuery$data;
  variables: organizationOnboarding_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CustomerDetails",
    "kind": "LinkedField",
    "name": "me",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "id",
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
    "name": "organizationOnboarding_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "organizationOnboarding_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "e82579942fdfe1977188c50c5f3cacd9",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_rootQuery",
    "operationKind": "query",
    "text": "query organizationOnboarding_rootQuery {\n  me {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "978c0d1025ca5c71500c9c32e92279f5";

export default node;
