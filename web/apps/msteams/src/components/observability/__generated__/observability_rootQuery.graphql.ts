/**
 * @generated SignedSource<<4a3e6e9b725b5f557c4229a9e84a9799>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type observability_rootQuery$variables = Record<PropertyKey, never>;
export type observability_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"logrocket_query">;
};
export type observability_rootQuery = {
  response: observability_rootQuery$data;
  variables: observability_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "observability_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "logrocket_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "observability_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v0/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "givenName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "middleName",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "familyName",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "faccd4f03c89c65153109936cf676308",
    "id": null,
    "metadata": {},
    "name": "observability_rootQuery",
    "operationKind": "query",
    "text": "query observability_rootQuery {\n  ...logrocket_query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n"
  }
};
})();

(node as any).hash = "0a55ecc1667f25b2db93a728abb5c761";

export default node;
