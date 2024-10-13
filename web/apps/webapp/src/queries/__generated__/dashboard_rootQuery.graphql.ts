/**
 * @generated SignedSource<<2575ed3fd3be1fa6974d2cc198a5764a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type dashboard_rootQuery$variables = Record<PropertyKey, never>;
export type dashboard_rootQuery$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
};
export type dashboard_rootQuery = {
  response: dashboard_rootQuery$data;
  variables: dashboard_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/)
],
v3 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "myLocations",
    "plural": true,
    "selections": [
      (v0/*: any*/),
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "LocationOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v2/*: any*/),
        "storageKey": null
      }
    ],
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "TeamDetails",
    "kind": "LinkedField",
    "name": "myTeams",
    "plural": true,
    "selections": [
      (v0/*: any*/),
      (v1/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "TeamOrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v2/*: any*/),
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
    "name": "dashboard_rootQuery",
    "selections": (v3/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "dashboard_rootQuery",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "c1d0508eba9f9966e5fe6673da2a638e",
    "id": null,
    "metadata": {},
    "name": "dashboard_rootQuery",
    "operationKind": "query",
    "text": "query dashboard_rootQuery {\n  myLocations {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n  myTeams {\n    id\n    name\n    organization {\n      uniqueId\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "f6482c6c872a8dab619431a550516778";

export default node;
