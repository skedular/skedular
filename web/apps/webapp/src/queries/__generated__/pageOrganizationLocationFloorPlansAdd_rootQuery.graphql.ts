/**
 * @generated SignedSource<<9d576cb9b51e1071518c978f12478f24>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageOrganizationLocationFloorPlansAdd_rootQuery$variables = {
  locationId: string;
};
export type pageOrganizationLocationFloorPlansAdd_rootQuery$data = {
  readonly location: {
    readonly name: string;
  } | null | undefined;
};
export type pageOrganizationLocationFloorPlansAdd_rootQuery = {
  response: pageOrganizationLocationFloorPlansAdd_rootQuery$data;
  variables: pageOrganizationLocationFloorPlansAdd_rootQuery$variables;
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
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationLocationFloorPlansAdd_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationLocationFloorPlansAdd_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
    "cacheID": "823b6a65b5af904c40cf540eb2b76a38",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationLocationFloorPlansAdd_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationLocationFloorPlansAdd_rootQuery(\n  $locationId: String!\n) {\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "52ffc11a47ccf42349731a03630687a9";

export default node;
