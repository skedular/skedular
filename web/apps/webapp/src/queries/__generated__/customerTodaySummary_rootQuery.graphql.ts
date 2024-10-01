/**
 * @generated SignedSource<<a65a57fc979ca31fe2cf9ec32d380a1c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type customerTodaySummary_rootQuery$variables = Record<PropertyKey, never>;
export type customerTodaySummary_rootQuery$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
};
export type customerTodaySummary_rootQuery = {
  response: customerTodaySummary_rootQuery$data;
  variables: customerTodaySummary_rootQuery$variables;
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
    "name": "customerTodaySummary_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "customerTodaySummary_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "0fa956ccaa66f3ecef5ad42e9a73d697",
    "id": null,
    "metadata": {},
    "name": "customerTodaySummary_rootQuery",
    "operationKind": "query",
    "text": "query customerTodaySummary_rootQuery {\n  me {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "68a8f771d3cb71024dd2dfdffceb23a7";

export default node;
