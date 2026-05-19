/**
 * @generated SignedSource<<b3b331404748aaaeaf5e0301474f28b8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type ResourceAvailabilityClassification = "AVAILABLE" | "BLOCKED" | "FULLY_BOOKED" | "OCCUPIED" | "PARTIALLY_BOOKED" | "UNAVAILABLE" | "%future added value";
export type ResourceAvailabilityOrderByField = "LOCATION_NAME" | "RESOURCE_NAME" | "RESOURCE_TYPE" | "ZONE_NAME" | "%future added value";
export type ResourceAvailabilityFilterInput = {
  date: any;
  floorId?: string | null | undefined;
  locationIds: ReadonlyArray<string>;
  organizationCustomDomain: string;
  resourceType?: string | null | undefined;
  statuses: ReadonlyArray<ResourceAvailabilityClassification>;
  zoneId?: string | null | undefined;
};
export type ResourceAvailabilityOrderByInput = {
  direction: OrderDirection;
  field: ResourceAvailabilityOrderByField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageAvailabilityDashboardQuery$variables = {
  filter: ResourceAvailabilityFilterInput;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  orderBy: ReadonlyArray<ResourceAvailabilityOrderByInput>;
  organizationCustomDomain: string;
};
export type pageAvailabilityDashboardQuery$data = {
  readonly resourceAvailabilityStatuses: ReadonlyArray<{
    readonly " $fragmentSpreads": FragmentRefs<"AvailabilityFilterBar_statuses">;
  }>;
  readonly resourceDayViews: {
    readonly " $fragmentSpreads": FragmentRefs<"AvailabilityDashboard_data">;
  };
  readonly " $fragmentSpreads": FragmentRefs<"AvailabilityFilterBar_locations">;
};
export type pageAvailabilityDashboardQuery = {
  response: pageAvailabilityDashboardQuery$data;
  variables: pageAvailabilityDashboardQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "filter"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "orderBy"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v4 = [
  {
    "kind": "Variable",
    "name": "filter",
    "variableName": "filter"
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "orderBy"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageAvailabilityDashboardQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*:: as any*/),
        "concreteType": "ResourceDayViewConnection",
        "kind": "LinkedField",
        "name": "resourceDayViews",
        "plural": false,
        "selections": [
          {
            "args": null,
            "kind": "FragmentSpread",
            "name": "AvailabilityDashboard_data"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "ResourceAvailabilityClassificationDetails",
        "kind": "LinkedField",
        "name": "resourceAvailabilityStatuses",
        "plural": true,
        "selections": [
          {
            "args": null,
            "kind": "FragmentSpread",
            "name": "AvailabilityFilterBar_statuses"
          }
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "AvailabilityFilterBar_locations"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*:: as any*/),
      (v0/*:: as any*/),
      (v2/*:: as any*/),
      (v1/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageAvailabilityDashboardQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*:: as any*/),
        "concreteType": "ResourceDayViewConnection",
        "kind": "LinkedField",
        "name": "resourceDayViews",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "subscriptionKey",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDayViewDetails",
            "kind": "LinkedField",
            "name": "items",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "resourceId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "resourceName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "resourceType",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "locationId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "locationName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "floorId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "floorName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zoneId",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zoneName",
                "storageKey": null
              },
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
                "name": "status",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "openingFrom",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "openingUntil",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "totalOpeningMinutes",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "bookedMinutes",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingWindowDetails",
                "kind": "LinkedField",
                "name": "bookingWindows",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "bookingId",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "from",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "until",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "isRecurring",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "isCheckedIn",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "bookedByName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "notes",
                    "storageKey": null
                  }
                ],
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
        "concreteType": "ResourceAvailabilityClassificationDetails",
        "kind": "LinkedField",
        "name": "resourceAvailabilityStatuses",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "type",
            "storageKey": null
          },
          (v5/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "organizationCustomDomain",
                "variableName": "organizationCustomDomain"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "id",
                    "storageKey": null
                  },
                  (v5/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "872ecb7000912076850bf07fc5df317a",
    "id": null,
    "metadata": {},
    "name": "pageAvailabilityDashboardQuery",
    "operationKind": "query",
    "text": "query pageAvailabilityDashboardQuery(\n  $organizationCustomDomain: String!\n  $filter: ResourceAvailabilityFilterInput!\n  $orderBy: [ResourceAvailabilityOrderByInput!]!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  resourceDayViews(filter: $filter, orderBy: $orderBy) {\n    ...AvailabilityDashboard_data\n  }\n  resourceAvailabilityStatuses {\n    ...AvailabilityFilterBar_statuses\n  }\n  ...AvailabilityFilterBar_locations\n}\n\nfragment AvailabilityDashboard_data on ResourceDayViewConnection {\n  subscriptionKey\n  ...ResourceDayViewList_result\n}\n\nfragment AvailabilityFilterBar_locations on Query {\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment AvailabilityFilterBar_statuses on ResourceAvailabilityClassificationDetails {\n  type\n  name\n}\n\nfragment ResourceDayViewCard_resourceDayView on ResourceDayViewDetails {\n  resourceId\n  resourceName\n  resourceType\n  locationId\n  locationName\n  floorId\n  floorName\n  zoneId\n  zoneName\n  date\n  status\n  openingFrom\n  openingUntil\n  totalOpeningMinutes\n  bookedMinutes\n  bookingWindows {\n    bookingId\n    from\n    until\n    isRecurring\n    isCheckedIn\n    bookedByName\n    notes\n  }\n}\n\nfragment ResourceDayViewList_result on ResourceDayViewConnection {\n  subscriptionKey\n  items {\n    resourceId\n    ...ResourceDayViewCard_resourceDayView\n  }\n}\n"
  }
};
})();

(node as any).hash = "d3a1c726df058f87955f922871b07d75";

export default node;
