/**
 * @generated SignedSource<<74aeebcd2cf2ab17b6efba2525717787>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type ResourceAvailabilityClassification = "AVAILABLE" | "BLOCKED" | "FULLY_BOOKED" | "OCCUPIED" | "PARTIALLY_BOOKED" | "UNAVAILABLE" | "%future added value";
export type ResourceAvailabilityFilterInput = {
  date: any;
  floorId?: string | null | undefined;
  locationIds: ReadonlyArray<string>;
  organizationCustomDomain: string;
  resourceType?: string | null | undefined;
  statuses: ReadonlyArray<ResourceAvailabilityClassification>;
  zoneId?: string | null | undefined;
};
export type AvailabilityDashboard_OnResourceAvailabilityChangedSubscription$variables = {
  filter: ResourceAvailabilityFilterInput;
  subscriptionKey: string;
};
export type AvailabilityDashboard_OnResourceAvailabilityChangedSubscription$data = {
  readonly resourceAvailability: {
    readonly " $fragmentSpreads": FragmentRefs<"ResourceDayViewList_result">;
  };
};
export type AvailabilityDashboard_OnResourceAvailabilityChangedSubscription = {
  response: AvailabilityDashboard_OnResourceAvailabilityChangedSubscription$data;
  variables: AvailabilityDashboard_OnResourceAvailabilityChangedSubscription$variables;
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
  "name": "subscriptionKey"
},
v2 = [
  {
    "kind": "Variable",
    "name": "filter",
    "variableName": "filter"
  },
  {
    "kind": "Variable",
    "name": "subscriptionKey",
    "variableName": "subscriptionKey"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "AvailabilityDashboard_OnResourceAvailabilityChangedSubscription",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "ResourceDayViewConnection",
        "kind": "LinkedField",
        "name": "resourceAvailability",
        "plural": false,
        "selections": [
          {
            "args": null,
            "kind": "FragmentSpread",
            "name": "ResourceDayViewList_result"
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Subscription",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "AvailabilityDashboard_OnResourceAvailabilityChangedSubscription",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "ResourceDayViewConnection",
        "kind": "LinkedField",
        "name": "resourceAvailability",
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
      }
    ]
  },
  "params": {
    "cacheID": "cf8d062de98e0970c5a3829ca531b12c",
    "id": null,
    "metadata": {},
    "name": "AvailabilityDashboard_OnResourceAvailabilityChangedSubscription",
    "operationKind": "subscription",
    "text": "subscription AvailabilityDashboard_OnResourceAvailabilityChangedSubscription(\n  $subscriptionKey: String!\n  $filter: ResourceAvailabilityFilterInput!\n) {\n  resourceAvailability(subscriptionKey: $subscriptionKey, filter: $filter) {\n    ...ResourceDayViewList_result\n  }\n}\n\nfragment ResourceDayViewCard_resourceDayView on ResourceDayViewDetails {\n  resourceId\n  resourceName\n  resourceType\n  locationId\n  locationName\n  floorId\n  floorName\n  zoneId\n  zoneName\n  date\n  status\n  openingFrom\n  openingUntil\n  totalOpeningMinutes\n  bookedMinutes\n  bookingWindows {\n    bookingId\n    from\n    until\n    isRecurring\n    isCheckedIn\n    bookedByName\n    notes\n  }\n}\n\nfragment ResourceDayViewList_result on ResourceDayViewConnection {\n  subscriptionKey\n  items {\n    resourceId\n    ...ResourceDayViewCard_resourceDayView\n  }\n}\n"
  }
};
})();

(node as any).hash = "4fb04e35aa8f8c0d97ff084bd35f93a2";

export default node;
