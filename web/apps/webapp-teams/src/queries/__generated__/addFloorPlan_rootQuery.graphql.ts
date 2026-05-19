/**
 * @generated SignedSource<<cb2890342b33164b62c4bc94a6eaf6b0>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagType = "AMENITY_ACCESS247" | "AMENITY_ACCESSIBLE_DESK_HEIGHT" | "AMENITY_ACCESSIBLE_RESTROOM" | "AMENITY_ACOUSTIC_TREATMENT" | "AMENITY_ADJUSTABLE_DESK" | "AMENITY_AFTER_HOURS_SUPPORT" | "AMENITY_AIR_CONDITIONING" | "AMENITY_AIR_PURIFIER" | "AMENITY_ALARM_SYSTEM" | "AMENITY_AMBIENT_LIGHTING_CONTROLS" | "AMENITY_BACKUP_INTERNET" | "AMENITY_BICYCLE_FRIENDLY_ACCESS" | "AMENITY_BIKE_STORAGE" | "AMENITY_BIOMETRIC_ENTRY" | "AMENITY_BLACKOUT_BLINDS" | "AMENITY_BOARDROOM_LAYOUT" | "AMENITY_BRAILLE_SIGNAGE" | "AMENITY_BREAKOUT_AREA" | "AMENITY_BUSINESS_HOURS_ACCESS" | "AMENITY_CANCELLATION_FLEXIBILITY" | "AMENITY_CATERING_SUPPORT" | "AMENITY_CCTV" | "AMENITY_CHECK_IN_REQUIRED" | "AMENITY_CHILDCARE_NEARBY" | "AMENITY_CLASSROOM_LAYOUT" | "AMENITY_CLEANING_SERVICE" | "AMENITY_COFFEE_MACHINE" | "AMENITY_COMMUNITY_MANAGER" | "AMENITY_CONFERENCE_TABLE" | "AMENITY_CONFIDENTIAL_MEETING_ROOM" | "AMENITY_DAILY_PASS" | "AMENITY_DEDICATED_VLAN" | "AMENITY_DEPOSIT_REQUIRED" | "AMENITY_DESK_PHONE" | "AMENITY_DISHWASHER" | "AMENITY_DUAL_MONITOR_SETUP" | "AMENITY_ELEVATOR_ACCESS" | "AMENITY_END_OF_TRIP_FACILITIES" | "AMENITY_ERGONOMIC_CHAIR" | "AMENITY_EVENT_STAFF_SUPPORT" | "AMENITY_EV_CHARGING" | "AMENITY_EXECUTIVE_CHAIR" | "AMENITY_FACILITIES_HELPDESK" | "AMENITY_FAMILY_ROOM" | "AMENITY_FILTERED_WATER" | "AMENITY_FIXED_DESK" | "AMENITY_FLIP_CHART" | "AMENITY_FOCUS_ROOM" | "AMENITY_FREIGHT_ELEVATOR" | "AMENITY_FRESH_AIR_VENTILATION" | "AMENITY_FRONT_DESK_SUPPORT" | "AMENITY_FULL_KITCHEN" | "AMENITY_GAME_LOUNGE_AREA" | "AMENITY_GARDEN_PATIO" | "AMENITY_GENDER_NEUTRAL_RESTROOM" | "AMENITY_GLASS_BOARD" | "AMENITY_GREEN_SCREEN" | "AMENITY_GUEST_NETWORK" | "AMENITY_GYM_ACCESS" | "AMENITY_HDMI_USB_C_CASTING" | "AMENITY_HEARING_LOOP" | "AMENITY_HEATING" | "AMENITY_HIGH_SPEED_WIFI" | "AMENITY_HOST_APPROVAL_REQUIRED" | "AMENITY_HOURLY_BOOKABLE" | "AMENITY_HUMIDITY_CONTROL" | "AMENITY_HYBRID_READY" | "AMENITY_IT_SUPPORT_ON_SITE" | "AMENITY_KEYBOARD_MOUSE_INCLUDED" | "AMENITY_KEYCARD_ENTRY" | "AMENITY_KITCHENETTE" | "AMENITY_LACTATION_ROOM" | "AMENITY_LAPTOP_STAND_AVAILABILITY" | "AMENITY_LARGE_DISPLAY" | "AMENITY_LEAD_TIME_REQUIREMENT" | "AMENITY_LIGHTING_RIG" | "AMENITY_LOADING_DOCK_ACCESS" | "AMENITY_LOCKABLE_PEDESTAL" | "AMENITY_LOCKABLE_ROOM" | "AMENITY_LOCKER_STORAGE" | "AMENITY_MAIL_HANDLING" | "AMENITY_MARKERS_INCLUDED" | "AMENITY_MAX_CAPACITY" | "AMENITY_MEDITATION_ROOM" | "AMENITY_MICROWAVE" | "AMENITY_MINIMUM_BOOKING_DURATION" | "AMENITY_MONITOR_ARM" | "AMENITY_MONITOR_DOCKING_STATION" | "AMENITY_MONTHLY_MEMBERSHIP" | "AMENITY_MOVABLE_SEATING" | "AMENITY_NAP_POD_ROOM" | "AMENITY_NATURAL_LIGHT" | "AMENITY_NETWORK_PRINTING" | "AMENITY_NOISE_CANCELLING_PANELS" | "AMENITY_ON_DEMAND_SETUP" | "AMENITY_ON_SITE_PARKING" | "AMENITY_ON_SITE_SECURITY" | "AMENITY_OPERABLE_WINDOWS" | "AMENITY_OUTDOOR_TERRACE" | "AMENITY_PACKAGE_RECEIVING" | "AMENITY_PA_SYSTEM" | "AMENITY_PET_FRIENDLY" | "AMENITY_PHONE_BOOTH" | "AMENITY_POWER_STRIPS" | "AMENITY_PRAYER_ROOM" | "AMENITY_PRIVACY_DESK_DIVIDER" | "AMENITY_PROJECTOR" | "AMENITY_PUBLIC_IP_OPTION" | "AMENITY_PUBLIC_TRANSIT_NEARBY" | "AMENITY_QUIET_ZONE" | "AMENITY_RECEPTION_DESK" | "AMENITY_RECORDING_ENABLED" | "AMENITY_REFRIGERATOR" | "AMENITY_REFUNDABLE_BOOKING" | "AMENITY_RESERVED_PARKING" | "AMENITY_RESTROOMS_NEARBY" | "AMENITY_ROOM_CAMERA" | "AMENITY_SAFETY_COMPLIANCE_KIT" | "AMENITY_SECURE_SSID" | "AMENITY_SERVICE_ANIMAL_READY" | "AMENITY_SHOWER_FACILITIES" | "AMENITY_SNACK_BAR" | "AMENITY_SOUNDPROOF_ROOM" | "AMENITY_SOUND_BOOTH" | "AMENITY_SPARKLING_WATER" | "AMENITY_SPEAKERPHONE" | "AMENITY_SPECIALIZED_TOOLS_EQUIPMENT" | "AMENITY_STAGE" | "AMENITY_STANDARD_POWER_OUTLETS" | "AMENITY_STANDING_DESK" | "AMENITY_STANDING_MEETING_ZONE" | "AMENITY_STEP_FREE_PATH" | "AMENITY_STORAGE_ROOM" | "AMENITY_STREET_PARKING_NEARBY" | "AMENITY_TASK_LAMP" | "AMENITY_TAXI_RIDESHARE_ZONE" | "AMENITY_TEA_STATION" | "AMENITY_TEMPERATURE_CONTROLS" | "AMENITY_UPS_BACKED_POWER" | "AMENITY_USB_A_PORTS" | "AMENITY_USB_C_PORTS" | "AMENITY_VIDEO_CONFERENCING_KIT" | "AMENITY_VISITOR_MANAGEMENT" | "AMENITY_VPN_SUPPORT" | "AMENITY_WASTE_RECYCLING_STATION" | "AMENITY_WELLNESS_ROOM" | "AMENITY_WHEELCHAIR_ACCESSIBLE_ENTRANCE" | "AMENITY_WHITEBOARD" | "AMENITY_WHITEBOARD_AT_DESK" | "AMENITY_WIDE_DOORWAYS" | "AMENITY_WIRED_ETHERNET" | "AMENITY_WIRELESS_CHARGING" | "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
export type ResourceOrderField = "NAME" | "%future added value";
export type ResourceOrderInput = {
  direction: OrderDirection;
  field: ResourceOrderField;
};
export type addFloorPlan_rootQuery$variables = {
  floorPlanId: string;
  locationId: string;
  resourcesSortingValues?: ReadonlyArray<ResourceOrderInput> | null | undefined;
};
export type addFloorPlan_rootQuery$data = {
  readonly deskResourceType: OrganizationTagType;
  readonly parkingResourceType: OrganizationTagType;
  readonly roomResourceType: OrganizationTagType;
  readonly " $fragmentSpreads": FragmentRefs<"addFloorPlan_resources_query">;
};
export type addFloorPlan_rootQuery = {
  response: addFloorPlan_rootQuery$data;
  variables: addFloorPlan_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "floorPlanId"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourcesSortingValues"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "deskResourceType",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "roomResourceType",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "parkingResourceType",
  "storageKey": null
},
v6 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "resourcesSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "floorPlanId",
        "variableName": "floorPlanId"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v10 = [
  (v7/*:: as any*/),
  (v8/*:: as any*/),
  (v9/*:: as any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addFloorPlan_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addFloorPlan_resources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*:: as any*/),
      (v0/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "addFloorPlan_rootQuery",
    "selections": [
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
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
            "args": (v6/*:: as any*/),
            "concreteType": "ConnectionOfResourceEdge",
            "kind": "LinkedField",
            "name": "resources",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourceEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "ResourceDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v7/*:: as any*/),
                      (v8/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "inactive",
                        "storageKey": null
                      },
                      (v9/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "capacity",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v10/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v10/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "productTags",
                        "plural": true,
                        "selections": (v10/*:: as any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "resourceType",
                        "plural": false,
                        "selections": [
                          (v7/*:: as any*/),
                          (v8/*:: as any*/),
                          (v9/*:: as any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "type",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "__typename",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "cursor",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "PageInfo",
                "kind": "LinkedField",
                "name": "pageInfo",
                "plural": false,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "endCursor",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasNextPage",
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
            "args": (v6/*:: as any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "addFloorPlanResourcesQuery_resources",
            "kind": "LinkedHandle",
            "name": "resources"
          },
          (v7/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "92b2b22f664e2dfd94aa168037f289f5",
    "id": null,
    "metadata": {},
    "name": "addFloorPlan_rootQuery",
    "operationKind": "query",
    "text": "query addFloorPlan_rootQuery(\n  $locationId: String!\n  $floorPlanId: String!\n  $resourcesSortingValues: [ResourceOrderInput!]\n) {\n  deskResourceType\n  roomResourceType\n  parkingResourceType\n  ...addFloorPlan_resources_query\n}\n\nfragment addFloorPlan_resources_query on Query {\n  location(id: $locationId) {\n    resources(where: {floorPlanId: $floorPlanId}, orderBy: $resourcesSortingValues) {\n      edges {\n        node {\n          id\n          name\n          inactive\n          color\n          capacity\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n          productTags {\n            id\n            name\n            color\n          }\n          resourceType {\n            id\n            name\n            color\n            type\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "e00b0dae00169f4b0a30a5cea25f3594";

export default node;
