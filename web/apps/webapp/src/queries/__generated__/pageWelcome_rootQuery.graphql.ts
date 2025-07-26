/**
 * @generated SignedSource<<7f257b76afea07a863b0aefe8371bc69>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageWelcome_rootQuery$variables = Record<PropertyKey, never>;
export type pageWelcome_rootQuery$data = {
  readonly me: {
    readonly id: string;
    readonly isOnboardingDone: boolean;
  };
};
export type pageWelcome_rootQuery = {
  response: pageWelcome_rootQuery$data;
  variables: pageWelcome_rootQuery$variables;
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
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "isOnboardingDone",
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
    "name": "pageWelcome_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageWelcome_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "0b249fc91af8d6cd23bdaa7478ae5fe4",
    "id": null,
    "metadata": {},
    "name": "pageWelcome_rootQuery",
    "operationKind": "query",
    "text": "query pageWelcome_rootQuery {\n  me {\n    id\n    isOnboardingDone\n  }\n}\n"
  }
};
})();

(node as any).hash = "2a942c1afa3148f463683f2ff6cfb8fb";

export default node;
