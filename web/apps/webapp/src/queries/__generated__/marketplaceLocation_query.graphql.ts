/**
 * @generated SignedSource<<2ffb08e84e6837ca5f78d475baab6c38>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type OrganizationTagType = "AMENITY_ACCESS247" | "AMENITY_ACCESSIBLE_DESK_HEIGHT" | "AMENITY_ACCESSIBLE_RESTROOM" | "AMENITY_ACOUSTIC_TREATMENT" | "AMENITY_ADJUSTABLE_DESK" | "AMENITY_AFTER_HOURS_SUPPORT" | "AMENITY_AIR_CONDITIONING" | "AMENITY_AIR_PURIFIER" | "AMENITY_ALARM_SYSTEM" | "AMENITY_AMBIENT_LIGHTING_CONTROLS" | "AMENITY_BACKUP_INTERNET" | "AMENITY_BICYCLE_FRIENDLY_ACCESS" | "AMENITY_BIKE_STORAGE" | "AMENITY_BIOMETRIC_ENTRY" | "AMENITY_BLACKOUT_BLINDS" | "AMENITY_BOARDROOM_LAYOUT" | "AMENITY_BRAILLE_SIGNAGE" | "AMENITY_BREAKOUT_AREA" | "AMENITY_BUSINESS_HOURS_ACCESS" | "AMENITY_CANCELLATION_FLEXIBILITY" | "AMENITY_CATERING_SUPPORT" | "AMENITY_CCTV" | "AMENITY_CHECK_IN_REQUIRED" | "AMENITY_CHILDCARE_NEARBY" | "AMENITY_CLASSROOM_LAYOUT" | "AMENITY_CLEANING_SERVICE" | "AMENITY_COFFEE_MACHINE" | "AMENITY_COMMUNITY_MANAGER" | "AMENITY_CONFERENCE_TABLE" | "AMENITY_CONFIDENTIAL_MEETING_ROOM" | "AMENITY_DAILY_PASS" | "AMENITY_DEDICATED_VLAN" | "AMENITY_DEPOSIT_REQUIRED" | "AMENITY_DESK_PHONE" | "AMENITY_DISHWASHER" | "AMENITY_DUAL_MONITOR_SETUP" | "AMENITY_ELEVATOR_ACCESS" | "AMENITY_END_OF_TRIP_FACILITIES" | "AMENITY_ERGONOMIC_CHAIR" | "AMENITY_EVENT_STAFF_SUPPORT" | "AMENITY_EV_CHARGING" | "AMENITY_EXECUTIVE_CHAIR" | "AMENITY_FACILITIES_HELPDESK" | "AMENITY_FAMILY_ROOM" | "AMENITY_FILTERED_WATER" | "AMENITY_FIXED_DESK" | "AMENITY_FLIP_CHART" | "AMENITY_FOCUS_ROOM" | "AMENITY_FREIGHT_ELEVATOR" | "AMENITY_FRESH_AIR_VENTILATION" | "AMENITY_FRONT_DESK_SUPPORT" | "AMENITY_FULL_KITCHEN" | "AMENITY_GAME_LOUNGE_AREA" | "AMENITY_GARDEN_PATIO" | "AMENITY_GENDER_NEUTRAL_RESTROOM" | "AMENITY_GLASS_BOARD" | "AMENITY_GREEN_SCREEN" | "AMENITY_GUEST_NETWORK" | "AMENITY_GYM_ACCESS" | "AMENITY_HDMI_USB_C_CASTING" | "AMENITY_HEARING_LOOP" | "AMENITY_HEATING" | "AMENITY_HIGH_SPEED_WIFI" | "AMENITY_HOST_APPROVAL_REQUIRED" | "AMENITY_HOURLY_BOOKABLE" | "AMENITY_HUMIDITY_CONTROL" | "AMENITY_HYBRID_READY" | "AMENITY_IT_SUPPORT_ON_SITE" | "AMENITY_KEYBOARD_MOUSE_INCLUDED" | "AMENITY_KEYCARD_ENTRY" | "AMENITY_KITCHENETTE" | "AMENITY_LACTATION_ROOM" | "AMENITY_LAPTOP_STAND_AVAILABILITY" | "AMENITY_LARGE_DISPLAY" | "AMENITY_LEAD_TIME_REQUIREMENT" | "AMENITY_LIGHTING_RIG" | "AMENITY_LOADING_DOCK_ACCESS" | "AMENITY_LOCKABLE_PEDESTAL" | "AMENITY_LOCKABLE_ROOM" | "AMENITY_LOCKER_STORAGE" | "AMENITY_MAIL_HANDLING" | "AMENITY_MARKERS_INCLUDED" | "AMENITY_MAX_CAPACITY" | "AMENITY_MEDITATION_ROOM" | "AMENITY_MICROWAVE" | "AMENITY_MINIMUM_BOOKING_DURATION" | "AMENITY_MONITOR_ARM" | "AMENITY_MONITOR_DOCKING_STATION" | "AMENITY_MONTHLY_MEMBERSHIP" | "AMENITY_MOVABLE_SEATING" | "AMENITY_NAP_POD_ROOM" | "AMENITY_NATURAL_LIGHT" | "AMENITY_NETWORK_PRINTING" | "AMENITY_NOISE_CANCELLING_PANELS" | "AMENITY_ON_DEMAND_SETUP" | "AMENITY_ON_SITE_PARKING" | "AMENITY_ON_SITE_SECURITY" | "AMENITY_OPERABLE_WINDOWS" | "AMENITY_OUTDOOR_TERRACE" | "AMENITY_PACKAGE_RECEIVING" | "AMENITY_PA_SYSTEM" | "AMENITY_PET_FRIENDLY" | "AMENITY_PHONE_BOOTH" | "AMENITY_POWER_STRIPS" | "AMENITY_PRAYER_ROOM" | "AMENITY_PRIVACY_DESK_DIVIDER" | "AMENITY_PROJECTOR" | "AMENITY_PUBLIC_IP_OPTION" | "AMENITY_PUBLIC_TRANSIT_NEARBY" | "AMENITY_QUIET_ZONE" | "AMENITY_RECEPTION_DESK" | "AMENITY_RECORDING_ENABLED" | "AMENITY_REFRIGERATOR" | "AMENITY_REFUNDABLE_BOOKING" | "AMENITY_RESERVED_PARKING" | "AMENITY_RESTROOMS_NEARBY" | "AMENITY_ROOM_CAMERA" | "AMENITY_SAFETY_COMPLIANCE_KIT" | "AMENITY_SECURE_SSID" | "AMENITY_SERVICE_ANIMAL_READY" | "AMENITY_SHOWER_FACILITIES" | "AMENITY_SNACK_BAR" | "AMENITY_SOUNDPROOF_ROOM" | "AMENITY_SOUND_BOOTH" | "AMENITY_SPARKLING_WATER" | "AMENITY_SPEAKERPHONE" | "AMENITY_SPECIALIZED_TOOLS_EQUIPMENT" | "AMENITY_STAGE" | "AMENITY_STANDARD_POWER_OUTLETS" | "AMENITY_STANDING_DESK" | "AMENITY_STANDING_MEETING_ZONE" | "AMENITY_STEP_FREE_PATH" | "AMENITY_STORAGE_ROOM" | "AMENITY_STREET_PARKING_NEARBY" | "AMENITY_TASK_LAMP" | "AMENITY_TAXI_RIDESHARE_ZONE" | "AMENITY_TEA_STATION" | "AMENITY_TEMPERATURE_CONTROLS" | "AMENITY_UPS_BACKED_POWER" | "AMENITY_USB_A_PORTS" | "AMENITY_USB_C_PORTS" | "AMENITY_VIDEO_CONFERENCING_KIT" | "AMENITY_VISITOR_MANAGEMENT" | "AMENITY_VPN_SUPPORT" | "AMENITY_WASTE_RECYCLING_STATION" | "AMENITY_WELLNESS_ROOM" | "AMENITY_WHEELCHAIR_ACCESSIBLE_ENTRANCE" | "AMENITY_WHITEBOARD" | "AMENITY_WHITEBOARD_AT_DESK" | "AMENITY_WIDE_DOORWAYS" | "AMENITY_WIRED_ETHERNET" | "AMENITY_WIRELESS_CHARGING" | "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocation_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly deskResourceType: OrganizationTagType;
  readonly floorPlans: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly id: string;
        readonly image: {
          readonly original: {
            readonly height: number | null | undefined;
            readonly url: string;
            readonly width: number | null | undefined;
          } | null | undefined;
        };
        readonly name: string;
        readonly resourceCount: number;
        readonly resourcePositions: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
          };
          readonly x: number;
          readonly y: number;
        }>;
      };
    }>;
  };
  readonly location: {
    readonly amenities: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
    readonly extraMetadata: {
      readonly areaRange: {
        readonly fromInSqm: string;
        readonly toInSqm: string;
      } | null | undefined;
      readonly contactDetails: {
        readonly contactEmails: ReadonlyArray<string> | null | undefined;
        readonly contactPeople: ReadonlyArray<string> | null | undefined;
        readonly contactPhones: ReadonlyArray<string> | null | undefined;
      } | null | undefined;
      readonly peopleCapacity: {
        readonly from: string;
        readonly to: string;
      } | null | undefined;
      readonly relatedImageLinks: ReadonlyArray<string> | null | undefined;
      readonly website: string | null | undefined;
    } | null | undefined;
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    }>;
    readonly id: string;
    readonly listingMetadata: {
      readonly about: string | null | undefined;
      readonly includedFeatures: ReadonlyArray<string> | null | undefined;
      readonly subTitle: string | null | undefined;
      readonly title: string | null | undefined;
    };
    readonly name: string;
    readonly openingHours: {
      readonly weekOpeningHours: {
        readonly friday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly monday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly saturday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly sunday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly thursday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly tuesday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
        readonly wednesday: {
          readonly closed: boolean;
          readonly from: string | null | undefined;
          readonly openAllDay: boolean;
          readonly until: string | null | undefined;
        };
      };
    };
    readonly organization: {
      readonly customDomain: string | null | undefined;
    };
    readonly physicalAddress: {
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly multilinesFormattedAddress: string | null | undefined;
    } | null | undefined;
    readonly products: ReadonlyArray<{
      readonly amenities: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly currency: {
        readonly type: Currency;
      };
      readonly featureImages: ReadonlyArray<{
        readonly original: {
          readonly url: string;
        } | null | undefined;
      }>;
      readonly id: string;
      readonly listingMetadata: {
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly pricingOptions: ReadonlyArray<{
        readonly id: string;
        readonly index: number;
        readonly isTaxInclusive: boolean;
        readonly listingMetadata: {
          readonly title: string | null | undefined;
        };
        readonly price: any;
        readonly purchaseCadence: ProductPricingCadence;
        readonly supportsSubscriptionAutoRenewal: boolean;
      }>;
      readonly productTags: ReadonlyArray<{
        readonly id: string;
      }>;
    }>;
    readonly resources?: {
      readonly edges: ReadonlyArray<{
        readonly node: {
          readonly color: string | null | undefined;
          readonly id: string;
          readonly inactive: boolean;
          readonly name: string;
          readonly productTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly resourceType: {
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
            readonly tagType: OrganizationTagType | null | undefined;
          };
        };
      }>;
    };
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly parkingResourceType: OrganizationTagType;
  readonly productPricingCadences: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCadence;
  }>;
  readonly roomResourceType: OrganizationTagType;
  readonly " $fragmentType": "marketplaceLocation_query";
};
export type marketplaceLocation_query$key = {
  readonly " $data"?: marketplaceLocation_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocation_query">;
};

import marketplaceLocation_refetchableFragment_graphql from './marketplaceLocation_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
],
v3 = {
  "kind": "Literal",
  "name": "orderBy",
  "value": [
    {
      "direction": "ASCENDING",
      "field": "NAME"
    }
  ]
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "url",
  "storageKey": null
},
v6 = [
  {
    "alias": null,
    "args": null,
    "concreteType": "CdnFile",
    "kind": "LinkedField",
    "name": "original",
    "plural": false,
    "selections": [
      (v5/*: any*/),
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
v7 = [
  (v4/*: any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "title",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "subTitle",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationTagDetails",
  "kind": "LinkedField",
  "name": "amenities",
  "plural": true,
  "selections": [
    (v4/*: any*/),
    (v1/*: any*/)
  ],
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "from",
  "storageKey": null
},
v12 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "closed",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "openAllDay",
    "storageKey": null
  },
  (v11/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "until",
    "storageKey": null
  }
],
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "defaultValue": false,
      "kind": "LocalArgument",
      "name": "floorPlanSelected"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "locationId"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "selectedFloorPlanId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": marketplaceLocation_refetchableFragment_graphql
    }
  },
  "name": "marketplaceLocation_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingCadenceDetails",
      "kind": "LinkedField",
      "name": "productPricingCadences",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "deskResourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "roomResourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "parkingResourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        (v3/*: any*/),
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "locationId",
              "variableName": "locationId"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfFloorPlanEdge",
      "kind": "LinkedField",
      "name": "floorPlans",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "FloorPlanEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "FloorPlanDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                (v4/*: any*/),
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "resourceCount",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CdnImageFile",
                  "kind": "LinkedField",
                  "name": "image",
                  "plural": false,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourcePositionDetails",
                  "kind": "LinkedField",
                  "name": "resourcePositions",
                  "plural": true,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "x",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "y",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "ResourceDetails",
                      "kind": "LinkedField",
                      "name": "resource",
                      "plural": false,
                      "selections": (v7/*: any*/),
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
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v2/*: any*/),
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
        (v4/*: any*/),
        (v1/*: any*/),
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
              "kind": "ScalarField",
              "name": "customDomain",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "ListingMetadata",
          "kind": "LinkedField",
          "name": "listingMetadata",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "about",
              "storageKey": null
            },
            (v8/*: any*/),
            (v9/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "includedFeatures",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "timezone",
          "storageKey": null
        },
        (v10/*: any*/),
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
              "concreteType": "ContactDetails",
              "kind": "LinkedField",
              "name": "contactDetails",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "contactPeople",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "contactEmails",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "contactPhones",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
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
                (v11/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "to",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "website",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "relatedImageLinks",
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
          "selections": (v6/*: any*/),
          "storageKey": null
        },
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
          "concreteType": "OpeningHours",
          "kind": "LinkedField",
          "name": "openingHours",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "WeekOpeningHours",
              "kind": "LinkedField",
              "name": "weekOpeningHours",
              "plural": false,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "monday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "tuesday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "wednesday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "thursday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "friday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "saturday",
                  "plural": false,
                  "selections": (v12/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OpeningHoursDetails",
                  "kind": "LinkedField",
                  "name": "sunday",
                  "plural": false,
                  "selections": (v12/*: any*/),
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
          "concreteType": "ProductDetails",
          "kind": "LinkedField",
          "name": "products",
          "plural": true,
          "selections": [
            (v4/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "ListingMetadata",
              "kind": "LinkedField",
              "name": "listingMetadata",
              "plural": false,
              "selections": [
                (v8/*: any*/),
                (v9/*: any*/)
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "productTags",
              "plural": true,
              "selections": (v7/*: any*/),
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
                  "name": "original",
                  "plural": false,
                  "selections": [
                    (v5/*: any*/)
                  ],
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CurrencyDetails",
              "kind": "LinkedField",
              "name": "currency",
              "plural": false,
              "selections": [
                (v0/*: any*/)
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "ProductPricing",
              "kind": "LinkedField",
              "name": "pricingOptions",
              "plural": true,
              "selections": [
                (v4/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "index",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ListingMetadata",
                  "kind": "LinkedField",
                  "name": "listingMetadata",
                  "plural": false,
                  "selections": [
                    (v8/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "purchaseCadence",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "price",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "isTaxInclusive",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "supportsSubscriptionAutoRenewal",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            (v10/*: any*/)
          ],
          "storageKey": null
        },
        {
          "condition": "floorPlanSelected",
          "kind": "Condition",
          "passingValue": true,
          "selections": [
            {
              "alias": null,
              "args": [
                (v3/*: any*/),
                {
                  "fields": [
                    {
                      "kind": "Variable",
                      "name": "floorPlanId",
                      "variableName": "selectedFloorPlanId"
                    }
                  ],
                  "kind": "ObjectValue",
                  "name": "where"
                }
              ],
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
                        (v4/*: any*/),
                        (v1/*: any*/),
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "inactive",
                          "storageKey": null
                        },
                        (v13/*: any*/),
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OrganizationTagDetails",
                          "kind": "LinkedField",
                          "name": "productTags",
                          "plural": true,
                          "selections": [
                            (v4/*: any*/),
                            (v1/*: any*/),
                            (v13/*: any*/)
                          ],
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
                            (v4/*: any*/),
                            (v1/*: any*/),
                            (v13/*: any*/),
                            {
                              "alias": null,
                              "args": null,
                              "kind": "ScalarField",
                              "name": "tagType",
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
              ],
              "storageKey": null
            }
          ]
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6bd26b6aa2313b20342e247cf7252d42";

export default node;
