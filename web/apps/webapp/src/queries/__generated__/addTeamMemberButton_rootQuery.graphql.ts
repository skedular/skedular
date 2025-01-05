/**
 * @generated SignedSource<<111ffa9c4150baf467c8b62c44a37b1f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type addTeamMemberButton_rootQuery$variables = Record<PropertyKey, never>;
export type addTeamMemberButton_rootQuery$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
};
export type addTeamMemberButton_rootQuery = {
  response: addTeamMemberButton_rootQuery$data;
  variables: addTeamMemberButton_rootQuery$variables;
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
    "name": "addTeamMemberButton_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "addTeamMemberButton_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "cd5329e287b786698d730d5e570b0a47",
    "id": null,
    "metadata": {},
    "name": "addTeamMemberButton_rootQuery",
    "operationKind": "query",
    "text": "query addTeamMemberButton_rootQuery {\n  me {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "896266aa581b8295465b6abd9ea6f7b5";

export default node;
