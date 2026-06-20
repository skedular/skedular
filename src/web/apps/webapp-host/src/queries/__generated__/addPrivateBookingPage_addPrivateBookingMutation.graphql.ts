/**
 * @generated SignedSource<<7b0548dc2a6cd8a0afe32bca8996a36c>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type SpacesQuotaReasonCode = "CUSTOM_CAPACITY_EXCEEDED" | "FREE_TIER_LIMIT_EXCEEDED" | "MISSING_OFFERING_STATE" | "NOT_SET" | "OUT_OF_PERIOD_EXCLUDED" | "PAID_TIER_LIMIT_EXCEEDED" | "TRIAL_EXPIRED" | "WITHIN_QUOTA" | "%future added value";
export type AddPrivateBookingInput = {
  category?: BookingCategory | null | undefined;
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  from: any;
  fullOpeningHoursDate?: any | null | undefined;
  id?: string | null | undefined;
  notes?: string | null | undefined;
  organizationCustomDomains?: ReadonlyArray<string> | null | undefined;
  organizationIds?: ReadonlyArray<string> | null | undefined;
  resourceIds?: ReadonlyArray<string> | null | undefined;
  teamIds?: ReadonlyArray<string> | null | undefined;
  until: any;
};
export type addPrivateBookingPage_addPrivateBookingMutation$variables = {
  input: AddPrivateBookingInput;
};
export type addPrivateBookingPage_addPrivateBookingMutation$data = {
  readonly addPrivateBooking: {
    readonly booking: {
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly from: any;
      readonly id: string;
      readonly involvedCustomers: ReadonlyArray<{
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly id: string;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
      }>;
      readonly involvedLocations: ReadonlyArray<{
        readonly name: string;
        readonly uniqueId: string;
      }>;
      readonly until: any;
    } | null | undefined;
    readonly quotaError: {
      readonly currentUsage: number;
      readonly errorCode: string;
      readonly quotaLimit: number;
      readonly reasonCode: {
        readonly name: string;
        readonly type: SpacesQuotaReasonCode;
      } | null | undefined;
      readonly upgradePlans: ReadonlyArray<{
        readonly availability: string;
        readonly name: string;
        readonly planCode: number;
        readonly priceDescription: string | null | undefined;
      }>;
    } | null | undefined;
  };
};
export type addPrivateBookingPage_addPrivateBookingMutation = {
  response: addPrivateBookingPage_addPrivateBookingMutation$data;
  variables: addPrivateBookingPage_addPrivateBookingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "BookingPayload",
    "kind": "LinkedField",
    "name": "addPrivateBooking",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingDetails",
        "kind": "LinkedField",
        "name": "booking",
        "plural": false,
        "selections": [
          (v1/*:: as any*/),
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
            "concreteType": "CustomerDetails",
            "kind": "LinkedField",
            "name": "involvedCustomers",
            "plural": true,
            "selections": [
              (v1/*:: as any*/),
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "givenName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "middleName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "familyName",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "photoUrl",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingResourceDetails",
            "kind": "LinkedField",
            "name": "bookingResources",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "ResourceDetails",
                "kind": "LinkedField",
                "name": "resource",
                "plural": false,
                "selections": [
                  (v1/*:: as any*/),
                  (v2/*:: as any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": [
                      (v1/*:: as any*/),
                      (v2/*:: as any*/)
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
        "concreteType": "BookingSpacesQuotaErrorDetails",
        "kind": "LinkedField",
        "name": "quotaError",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "errorCode",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "SpacesQuotaReasonCodeDetails",
            "kind": "LinkedField",
            "name": "reasonCode",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "type",
                "storageKey": null
              },
              (v2/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "currentUsage",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "quotaLimit",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "UpgradePlanDetails",
            "kind": "LinkedField",
            "name": "upgradePlans",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "planCode",
                "storageKey": null
              },
              (v2/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "availability",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "priceDescription",
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "addPrivateBookingPage_addPrivateBookingMutation",
    "selections": (v3/*:: as any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "addPrivateBookingPage_addPrivateBookingMutation",
    "selections": (v3/*:: as any*/)
  },
  "params": {
    "cacheID": "9849891f7b87570dde896af3be243e34",
    "id": null,
    "metadata": {},
    "name": "addPrivateBookingPage_addPrivateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation addPrivateBookingPage_addPrivateBookingMutation(\n  $input: AddPrivateBookingInput!\n) {\n  addPrivateBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedLocations {\n        uniqueId\n        name\n      }\n      bookingResources {\n        resource {\n          id\n          name\n          zones {\n            id\n            name\n          }\n        }\n      }\n    }\n    quotaError {\n      errorCode\n      reasonCode {\n        type\n        name\n      }\n      currentUsage\n      quotaLimit\n      upgradePlans {\n        planCode\n        name\n        availability\n        priceDescription\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "4207461e7e8a47b4f954ef7b8e8e8841";

export default node;
