/**
 * @generated SignedSource<<3521e0c78ddbc10390097562d8a5f2e0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsightRoot_rootQuery$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsightRoot_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_query">;
};
export type locationDeskOccupancyInsightRoot_rootQuery = {
  response: locationDeskOccupancyInsightRoot_rootQuery$data;
  variables: locationDeskOccupancyInsightRoot_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "from"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "to"
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDeskOccupancyInsight_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "from",
            "variableName": "from"
          },
          {
            "kind": "Variable",
            "name": "locationId",
            "variableName": "locationId"
          },
          {
            "kind": "Variable",
            "name": "until",
            "variableName": "to"
          }
        ],
        "concreteType": "LocationAnalytics",
        "kind": "LinkedField",
        "name": "locationAnalytics",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDesksOccupancyPercentage",
            "kind": "LinkedField",
            "name": "desksOccupancyPercentage",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "date",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "percentage",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          },
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
    "cacheID": "6098682be6336014f29b825a68c78b13",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsightRoot_rootQuery",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsightRoot_rootQuery(\n  $locationId: String!\n  $from: DateTime!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_query\n}\n\nfragment locationDeskOccupancyInsight_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a49cce66764fad8d0cf79259a689f2a3";

export default node;
