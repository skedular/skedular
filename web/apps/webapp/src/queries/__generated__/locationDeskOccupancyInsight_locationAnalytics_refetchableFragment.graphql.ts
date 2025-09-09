/**
 * @generated SignedSource<<26d8a96ce222582cf7299de015abda65>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$variables = {
  from: any;
  locationId: string;
  to: any;
};
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationDeskOccupancyInsight_locationAnalytics_query">;
};
export type locationDeskOccupancyInsight_locationAnalytics_refetchableFragment = {
  response: locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$data;
  variables: locationDeskOccupancyInsight_locationAnalytics_refetchableFragment$variables;
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
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationDeskOccupancyInsight_locationAnalytics_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "selections": [
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
            "args": [
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "from"
              },
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "to"
              }
            ],
            "concreteType": "LocationAnalytics",
            "kind": "LinkedField",
            "name": "analytics",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "DesksOccupancyPercentage",
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
    "cacheID": "364bd3ea6e2aeb44af4712c14443c40e",
    "id": null,
    "metadata": {},
    "name": "locationDeskOccupancyInsight_locationAnalytics_refetchableFragment",
    "operationKind": "query",
    "text": "query locationDeskOccupancyInsight_locationAnalytics_refetchableFragment(\n  $from: DateTime!\n  $locationId: String!\n  $to: DateTime!\n) {\n  ...locationDeskOccupancyInsight_locationAnalytics_query\n}\n\nfragment locationDeskOccupancyInsight_locationAnalytics_query on Query {\n  location(id: $locationId) {\n    analytics(from: $from, until: $to) {\n      desksOccupancyPercentage {\n        date\n        percentage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "0fbcfcd225e53c4a4a346290e4bb67bc";

export default node;
