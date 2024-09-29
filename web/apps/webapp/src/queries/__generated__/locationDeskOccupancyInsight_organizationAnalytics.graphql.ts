/**
 * @generated SignedSource<<4e27a3df02bbe60ebd447c16eb76298a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_organizationAnalytics$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsight_organizationAnalytics$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_query">;
};
export type locationDeskOccupancyInsight_organizationAnalytics = {
  response: locationDeskOccupancyInsight_organizationAnalytics$data;
  variables: locationDeskOccupancyInsight_organizationAnalytics$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "from"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "to"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
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
    "cacheID": "9df93fbf45b4e62c44ee85820e0618c2",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsight_organizationAnalytics",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsight_organizationAnalytics(\n  $from: DateTime!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_query\n}\n\nfragment locationDeskOccupancyInsight_query on Query {\n  locationAnalytics(locationId: $locationId, from: $from, until: $to) {\n    desksOccupancyPercentage {\n      date\n      percentage\n    }\n  }\n  location(id: $locationId) {\n    name\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "76cc9d1819026aa182d92feb9d172a77";

export default node;
