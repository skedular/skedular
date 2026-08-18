/**
 * @generated SignedSource<<befade0f9412a12416773f5c8b13244f>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationTagType = "AMENITY_ACCESS247" | "AMENITY_ACCESSIBLE_DESK_HEIGHT" | "AMENITY_ACCESSIBLE_RESTROOM" | "AMENITY_ACOUSTIC_TREATMENT" | "AMENITY_ADJUSTABLE_DESK" | "AMENITY_AFTER_HOURS_SUPPORT" | "AMENITY_AIR_CONDITIONING" | "AMENITY_AIR_PURIFIER" | "AMENITY_ALARM_SYSTEM" | "AMENITY_AMBIENT_LIGHTING_CONTROLS" | "AMENITY_BACKUP_INTERNET" | "AMENITY_BICYCLE_FRIENDLY_ACCESS" | "AMENITY_BIKE_STORAGE" | "AMENITY_BIOMETRIC_ENTRY" | "AMENITY_BLACKOUT_BLINDS" | "AMENITY_BOARDROOM_LAYOUT" | "AMENITY_BRAILLE_SIGNAGE" | "AMENITY_BREAKOUT_AREA" | "AMENITY_BUSINESS_HOURS_ACCESS" | "AMENITY_CANCELLATION_FLEXIBILITY" | "AMENITY_CATERING_SUPPORT" | "AMENITY_CCTV" | "AMENITY_CHECK_IN_REQUIRED" | "AMENITY_CHILDCARE_NEARBY" | "AMENITY_CLASSROOM_LAYOUT" | "AMENITY_CLEANING_SERVICE" | "AMENITY_COFFEE_MACHINE" | "AMENITY_COMMUNITY_MANAGER" | "AMENITY_CONFERENCE_TABLE" | "AMENITY_CONFIDENTIAL_MEETING_ROOM" | "AMENITY_DAILY_PASS" | "AMENITY_DEDICATED_VLAN" | "AMENITY_DEPOSIT_REQUIRED" | "AMENITY_DESK_PHONE" | "AMENITY_DISHWASHER" | "AMENITY_DUAL_MONITOR_SETUP" | "AMENITY_ELEVATOR_ACCESS" | "AMENITY_END_OF_TRIP_FACILITIES" | "AMENITY_ERGONOMIC_CHAIR" | "AMENITY_EVENT_STAFF_SUPPORT" | "AMENITY_EV_CHARGING" | "AMENITY_EXECUTIVE_CHAIR" | "AMENITY_FACILITIES_HELPDESK" | "AMENITY_FAMILY_ROOM" | "AMENITY_FILTERED_WATER" | "AMENITY_FIXED_DESK" | "AMENITY_FLIP_CHART" | "AMENITY_FOCUS_ROOM" | "AMENITY_FREIGHT_ELEVATOR" | "AMENITY_FRESH_AIR_VENTILATION" | "AMENITY_FRONT_DESK_SUPPORT" | "AMENITY_FULL_KITCHEN" | "AMENITY_GAME_LOUNGE_AREA" | "AMENITY_GARDEN_PATIO" | "AMENITY_GENDER_NEUTRAL_RESTROOM" | "AMENITY_GLASS_BOARD" | "AMENITY_GREEN_SCREEN" | "AMENITY_GUEST_NETWORK" | "AMENITY_GYM_ACCESS" | "AMENITY_HDMI_USB_C_CASTING" | "AMENITY_HEARING_LOOP" | "AMENITY_HEATING" | "AMENITY_HIGH_SPEED_WIFI" | "AMENITY_HOST_APPROVAL_REQUIRED" | "AMENITY_HOURLY_BOOKABLE" | "AMENITY_HUMIDITY_CONTROL" | "AMENITY_HYBRID_READY" | "AMENITY_IT_SUPPORT_ON_SITE" | "AMENITY_KEYBOARD_MOUSE_INCLUDED" | "AMENITY_KEYCARD_ENTRY" | "AMENITY_KITCHENETTE" | "AMENITY_LACTATION_ROOM" | "AMENITY_LAPTOP_STAND_AVAILABILITY" | "AMENITY_LARGE_DISPLAY" | "AMENITY_LEAD_TIME_REQUIREMENT" | "AMENITY_LIGHTING_RIG" | "AMENITY_LOADING_DOCK_ACCESS" | "AMENITY_LOCKABLE_PEDESTAL" | "AMENITY_LOCKABLE_ROOM" | "AMENITY_LOCKER_STORAGE" | "AMENITY_MAIL_HANDLING" | "AMENITY_MARKERS_INCLUDED" | "AMENITY_MAX_CAPACITY" | "AMENITY_MEDITATION_ROOM" | "AMENITY_MICROWAVE" | "AMENITY_MINIMUM_BOOKING_DURATION" | "AMENITY_MONITOR_ARM" | "AMENITY_MONITOR_DOCKING_STATION" | "AMENITY_MONTHLY_MEMBERSHIP" | "AMENITY_MOVABLE_SEATING" | "AMENITY_NAP_POD_ROOM" | "AMENITY_NATURAL_LIGHT" | "AMENITY_NETWORK_PRINTING" | "AMENITY_NOISE_CANCELLING_PANELS" | "AMENITY_ON_DEMAND_SETUP" | "AMENITY_ON_SITE_PARKING" | "AMENITY_ON_SITE_SECURITY" | "AMENITY_OPERABLE_WINDOWS" | "AMENITY_OUTDOOR_TERRACE" | "AMENITY_PACKAGE_RECEIVING" | "AMENITY_PA_SYSTEM" | "AMENITY_PET_FRIENDLY" | "AMENITY_PHONE_BOOTH" | "AMENITY_POWER_STRIPS" | "AMENITY_PRAYER_ROOM" | "AMENITY_PRIVACY_DESK_DIVIDER" | "AMENITY_PROJECTOR" | "AMENITY_PUBLIC_IP_OPTION" | "AMENITY_PUBLIC_TRANSIT_NEARBY" | "AMENITY_QUIET_ZONE" | "AMENITY_RECEPTION_DESK" | "AMENITY_RECORDING_ENABLED" | "AMENITY_REFRIGERATOR" | "AMENITY_REFUNDABLE_BOOKING" | "AMENITY_RESERVED_PARKING" | "AMENITY_RESTROOMS_NEARBY" | "AMENITY_ROOM_CAMERA" | "AMENITY_SAFETY_COMPLIANCE_KIT" | "AMENITY_SECURE_SSID" | "AMENITY_SERVICE_ANIMAL_READY" | "AMENITY_SHOWER_FACILITIES" | "AMENITY_SNACK_BAR" | "AMENITY_SOUNDPROOF_ROOM" | "AMENITY_SOUND_BOOTH" | "AMENITY_SPARKLING_WATER" | "AMENITY_SPEAKERPHONE" | "AMENITY_SPECIALIZED_TOOLS_EQUIPMENT" | "AMENITY_STAGE" | "AMENITY_STANDARD_POWER_OUTLETS" | "AMENITY_STANDING_DESK" | "AMENITY_STANDING_MEETING_ZONE" | "AMENITY_STEP_FREE_PATH" | "AMENITY_STORAGE_ROOM" | "AMENITY_STREET_PARKING_NEARBY" | "AMENITY_TASK_LAMP" | "AMENITY_TAXI_RIDESHARE_ZONE" | "AMENITY_TEA_STATION" | "AMENITY_TEMPERATURE_CONTROLS" | "AMENITY_UPS_BACKED_POWER" | "AMENITY_USB_A_PORTS" | "AMENITY_USB_C_PORTS" | "AMENITY_VIDEO_CONFERENCING_KIT" | "AMENITY_VISITOR_MANAGEMENT" | "AMENITY_VPN_SUPPORT" | "AMENITY_WASTE_RECYCLING_STATION" | "AMENITY_WELLNESS_ROOM" | "AMENITY_WHEELCHAIR_ACCESSIBLE_ENTRANCE" | "AMENITY_WHITEBOARD" | "AMENITY_WHITEBOARD_AT_DESK" | "AMENITY_WIDE_DOORWAYS" | "AMENITY_WIRED_ETHERNET" | "AMENITY_WIRELESS_CHARGING" | "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_ENTIRE_LOCATION" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
export type PolygonInput = {
  northEast: PointCoordinatesInput;
  southWest: PointCoordinatesInput;
};
export type PointCoordinatesInput = {
  latitude: number;
  longitude: number;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageHome_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  resourceTypeToFilterWith?: OrganizationTagType | null | undefined;
  searchBoundaries?: PolygonInput | null | undefined;
  userSignedIn: boolean;
};
export type pageHome_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocations_locations_query" | "marketplaceLocations_query">;
};
export type pageHome_rootQuery = {
  response: pageHome_rootQuery$data;
  variables: pageHome_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "resourceTypeToFilterWith"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "searchBoundaries"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "userSignedIn"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "resourceType",
        "variableName": "resourceTypeToFilterWith"
      },
      {
        "kind": "Variable",
        "name": "searchBoundaries",
        "variableName": "searchBoundaries"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v6 = {
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
    "name": "pageHome_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "marketplaceLocations_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "marketplaceLocations_locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v3/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "pageHome_rootQuery",
    "selections": [
      {
        "condition": "userSignedIn",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "me",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "favouriteLocations",
                "plural": true,
                "selections": [
                  (v4/*:: as any*/)
                ],
                "storageKey": null
              },
              (v4/*:: as any*/)
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "marketplaceLocations",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalCount",
            "storageKey": null
          },
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
                  (v4/*:: as any*/),
                  (v6/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationPhysicalAddressDetails",
                    "kind": "LinkedField",
                    "name": "physicalAddress",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "longitude",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "latitude",
                        "storageKey": null
                      },
                      (v4/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "multilinesFormattedAddress",
                        "storageKey": null
                      }
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "OrganizationTypeDetails",
                        "kind": "LinkedField",
                        "name": "type",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "type",
                            "storageKey": null
                          },
                          (v6/*:: as any*/)
                        ],
                        "storageKey": null
                      },
                      (v4/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "SpacesPublicBookingAvailabilityDetails",
                        "kind": "LinkedField",
                        "name": "spacesPublicBookingAvailability",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "available",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "message",
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
                    "concreteType": "LocationExtraMetadata",
                    "kind": "LinkedField",
                    "name": "extraMetadata",
                    "plural": false,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "AreaRange",
                        "kind": "LinkedField",
                        "name": "areaRange",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "fromInSqm",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "toInSqm",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "PeopleCapacity",
                        "kind": "LinkedField",
                        "name": "peopleCapacity",
                        "plural": false,
                        "selections": [
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
                            "name": "to",
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
                    "concreteType": "CdnImageFile",
                    "kind": "LinkedField",
                    "name": "featureImages",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CdnFile",
                        "kind": "LinkedField",
                        "name": "thumbnail",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "url",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "height",
                            "storageKey": null
                          },
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "width",
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
          },
          {
            "kind": "ClientExtension",
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "__id",
                "storageKey": null
              }
            ]
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locations_marketplaceLocations",
        "kind": "LinkedHandle",
        "name": "marketplaceLocations"
      }
    ]
  },
  "params": {
    "cacheID": "c2396741b0476902dc98500f4e034960",
    "id": null,
    "metadata": {},
    "name": "pageHome_rootQuery",
    "operationKind": "query",
    "text": "query pageHome_rootQuery(\n  $searchBoundaries: PolygonInput\n  $locationsSortingValues: [LocationOrderInput!]\n  $resourceTypeToFilterWith: OrganizationTagType\n  $userSignedIn: Boolean!\n) {\n  ...marketplaceLocations_query\n  ...marketplaceLocations_locations_query\n}\n\nfragment marketplaceLocationCard_LocationDetails on LocationDetails {\n  id\n  name\n  organization {\n    type {\n      type\n      name\n    }\n    spacesPublicBookingAvailability {\n      available\n      message\n    }\n    id\n  }\n  extraMetadata {\n    areaRange {\n      fromInSqm\n      toInSqm\n    }\n    peopleCapacity {\n      from\n      to\n    }\n  }\n  physicalAddress {\n    multilinesFormattedAddress\n    id\n  }\n  featureImages {\n    thumbnail {\n      url\n      height\n      width\n    }\n  }\n}\n\nfragment marketplaceLocationCard_query on Query {\n  me @include(if: $userSignedIn) {\n    favouriteLocations {\n      id\n    }\n    id\n  }\n}\n\nfragment marketplaceLocations_locations_query on Query {\n  marketplaceLocations(where: {searchBoundaries: $searchBoundaries, resourceType: $resourceTypeToFilterWith}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        physicalAddress {\n          longitude\n          latitude\n          id\n        }\n        organization {\n          type {\n            type\n          }\n          id\n        }\n        ...marketplaceLocationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment marketplaceLocations_query on Query {\n  ...marketplaceLocationCard_query\n}\n"
  }
};
})();

(node as any).hash = "e73b1eb6d0d82fa11163d13f2580ca05";

export default node;
