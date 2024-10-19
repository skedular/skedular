/**
 * @generated SignedSource<<35ab5763228db3b5ed7e31679fd9c0b3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type location_rootQuery$variables = {
  locationId: string;
  organizationExists: boolean;
  organizationId: string;
};
export type location_rootQuery$data = {
  readonly location: {
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
};
export type location_rootQuery = {
  response: location_rootQuery$data;
  variables: location_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = [
  {
    "condition": "organizationExists",
    "kind": "Condition",
    "passingValue": true,
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "organizationId"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "locationId"
      }
    ],
    "concreteType": "LocationDetails",
    "kind": "LinkedField",
    "name": "location",
    "plural": false,
    "selections": [
      (v3/*: any*/),
      (v4/*: any*/),
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canViewAnalytics",
        "storageKey": null
      },
      {
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
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "location_rootQuery",
    "selections": (v5/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "location_rootQuery",
    "selections": (v5/*: any*/)
  },
  "params": {
    "cacheID": "5e4055c3fa9bc8e9f49471440c1faaaf",
    "id": null,
    "metadata": {},
    "name": "location_rootQuery",
    "operationKind": "query",
    "text": "query location_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $locationId: String!\n) {\n  organization(id: $organizationId) @include(if: $organizationExists) {\n    id\n    name\n  }\n  location(id: $locationId) {\n    id\n    name\n    canViewAnalytics\n    organization {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "788463462fec13650d59e3a4b5e64656";

export default node;
