/**
 * @generated SignedSource<<f9316b8f940961b062f7183d16c9eb0d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type location_rootQuery$variables = {
  locationId: string;
};
export type location_rootQuery$data = {
  readonly location: {
    readonly canViewAnalytics: boolean;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
};
export type location_rootQuery = {
  response: location_rootQuery$data;
  variables: location_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "locationId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "concreteType": "LocationOrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "uniqueId",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "location_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          (v3/*: any*/),
          (v4/*: any*/),
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
    ]
  },
  "params": {
    "cacheID": "4750c952b10465513142177661464b46",
    "id": null,
    "metadata": {},
    "name": "location_rootQuery",
    "operationKind": "query",
    "text": "query location_rootQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    name\n    canViewAnalytics\n    organization {\n      uniqueId\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "c72122cebee0c14168339eca1a94c8cf";

export default node;
