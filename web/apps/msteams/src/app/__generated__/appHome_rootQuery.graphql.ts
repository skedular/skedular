/**
 * @generated SignedSource<<89f9308d1c908504cd4bbdd733a606a2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
export type appHome_rootQuery$variables = Record<PropertyKey, never>;
export type appHome_rootQuery$data = {
  readonly msTeamsCustomerRecordSynced: boolean;
  readonly msTeamsVersion: {
    readonly major: number;
  };
};
export type appHome_rootQuery = {
  response: appHome_rootQuery$data;
  variables: appHome_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "msTeamsCustomerRecordSynced",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "Version",
    "kind": "LinkedField",
    "name": "msTeamsVersion",
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
    "name": "appHome_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "appHome_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "7014b242ff0df5f9be877bb1c01e7682",
    "id": null,
    "metadata": {},
    "name": "appHome_rootQuery",
    "operationKind": "query",
    "text": "query appHome_rootQuery {\n  msTeamsCustomerRecordSynced\n  msTeamsVersion {\n    major\n  }\n}\n"
  }
};
})();

(node as any).hash = "979847c5807d6a423a177b1e935fafd0";

export default node;
