/**
 * @generated SignedSource<<6492e0506764a8fd7fa6e7fb32639baa>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type Currency = "NZD" | "USD" | "%future added value";
export type MarketplaceRefundStatus = "APPROVED" | "CANCELLED" | "COMPLETED" | "FAILED" | "PROCESSING" | "PROVIDER_PENDING" | "RECONCILIATION_REQUIRED" | "REJECTED" | "REQUESTED" | "UNDER_REVIEW" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
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
export type resourceCard_addPrivateBookingMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: AddPrivateBookingInput;
};
export type resourceCard_addPrivateBookingMutation$data = {
  readonly addPrivateBooking: {
    readonly booking: {
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly category: {
        readonly category: BookingCategory;
        readonly name: string;
      };
      readonly channel: {
        readonly channel: BookingChannel;
        readonly name: string;
      };
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
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly involvedTeams: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly marketplaceBooking: {
        readonly id: string;
        readonly invoiceUrl: string | null | undefined;
        readonly isPaymentRequired: boolean;
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
        readonly refund: {
          readonly canProcessInXero: boolean;
          readonly currency: {
            readonly name: string;
            readonly type: Currency;
          } | null | undefined;
          readonly currencyToDisplay: string;
          readonly externalRefundNumber: string | null | undefined;
          readonly id: string;
          readonly lastError: string | null | undefined;
          readonly lastProcessedAt: any | null | undefined;
          readonly reason: string | null | undefined;
          readonly refundAmount: any | null | undefined;
          readonly refundPercentage: number;
          readonly requestedAt: any;
          readonly requestedByCustomerName: string | null | undefined;
          readonly status: {
            readonly name: string;
            readonly type: MarketplaceRefundStatus;
          };
          readonly xeroProcessingBlockedReason: string | null | undefined;
        } | null | undefined;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly recurringBooking: {
        readonly endDate: any | null | undefined;
        readonly frequency: {
          readonly name: string;
        };
        readonly id: string;
        readonly marketplaceBooking: {
          readonly id: string;
        } | null | undefined;
        readonly startDate: any;
      } | null | undefined;
      readonly until: any;
    } | null | undefined;
    readonly quotaError: {
      readonly errorCode: string;
      readonly reasonCode: {
        readonly name: string;
        readonly type: SpacesQuotaReasonCode;
      } | null | undefined;
    } | null | undefined;
  };
};
export type resourceCard_addPrivateBookingMutation$rawResponse = {
  readonly addPrivateBooking: {
    readonly booking: {
      readonly bookingResources: ReadonlyArray<{
        readonly resource: {
          readonly color: string | null | undefined;
          readonly customTags: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
          readonly id: string;
          readonly name: string;
          readonly zones: ReadonlyArray<{
            readonly color: string | null | undefined;
            readonly id: string;
            readonly name: string;
          }>;
        };
      }>;
      readonly category: {
        readonly category: BookingCategory;
        readonly name: string;
      };
      readonly channel: {
        readonly channel: BookingChannel;
        readonly name: string;
      };
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
      readonly involvedOrganizations: ReadonlyArray<{
        readonly id: string;
      }>;
      readonly involvedTeams: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly marketplaceBooking: {
        readonly id: string;
        readonly invoiceUrl: string | null | undefined;
        readonly isPaymentRequired: boolean;
        readonly paymentStatus: {
          readonly name: string;
          readonly type: PaymentStatus;
        };
        readonly refund: {
          readonly canProcessInXero: boolean;
          readonly currency: {
            readonly name: string;
            readonly type: Currency;
          } | null | undefined;
          readonly currencyToDisplay: string;
          readonly externalRefundNumber: string | null | undefined;
          readonly id: string;
          readonly lastError: string | null | undefined;
          readonly lastProcessedAt: any | null | undefined;
          readonly reason: string | null | undefined;
          readonly refundAmount: any | null | undefined;
          readonly refundPercentage: number;
          readonly requestedAt: any;
          readonly requestedByCustomerName: string | null | undefined;
          readonly status: {
            readonly name: string;
            readonly type: MarketplaceRefundStatus;
          };
          readonly xeroProcessingBlockedReason: string | null | undefined;
        } | null | undefined;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly recurringBooking: {
        readonly endDate: any | null | undefined;
        readonly frequency: {
          readonly name: string;
        };
        readonly id: string;
        readonly marketplaceBooking: {
          readonly id: string;
        } | null | undefined;
        readonly startDate: any;
      } | null | undefined;
      readonly until: any;
    } | null | undefined;
    readonly quotaError: {
      readonly errorCode: string;
      readonly reasonCode: {
        readonly name: string;
        readonly type: SpacesQuotaReasonCode;
      } | null | undefined;
    } | null | undefined;
  };
};
export type resourceCard_addPrivateBookingMutation = {
  rawResponse: resourceCard_addPrivateBookingMutation$rawResponse;
  response: resourceCard_addPrivateBookingMutation$data;
  variables: resourceCard_addPrivateBookingMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = [
  (v2/*:: as any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v6 = [
  (v2/*:: as any*/),
  (v3/*:: as any*/),
  (v5/*:: as any*/)
],
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v3/*:: as any*/)
],
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "BookingDetails",
  "kind": "LinkedField",
  "name": "booking",
  "plural": false,
  "selections": [
    (v2/*:: as any*/),
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
      "name": "notes",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingChannelDetails",
      "kind": "LinkedField",
      "name": "channel",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "channel",
          "storageKey": null
        },
        (v3/*:: as any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingCategoryDetails",
      "kind": "LinkedField",
      "name": "category",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "category",
          "storageKey": null
        },
        (v3/*:: as any*/)
      ],
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
        (v2/*:: as any*/),
        (v3/*:: as any*/),
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "involvedOrganizations",
      "plural": true,
      "selections": (v4/*:: as any*/),
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
        (v3/*:: as any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "involvedTeams",
      "plural": true,
      "selections": [
        (v2/*:: as any*/),
        (v3/*:: as any*/)
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
            (v2/*:: as any*/),
            (v3/*:: as any*/),
            (v5/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v6/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationTagDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v6/*:: as any*/),
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
      "concreteType": "MarketplaceBookingDetails",
      "kind": "LinkedField",
      "name": "marketplaceBooking",
      "plural": false,
      "selections": [
        (v2/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isPaymentRequired",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PaymentStatusDetails",
          "kind": "LinkedField",
          "name": "paymentStatus",
          "plural": false,
          "selections": (v7/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "invoiceUrl",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceRefundDetails",
          "kind": "LinkedField",
          "name": "refund",
          "plural": false,
          "selections": [
            (v2/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "CurrencyDetails",
              "kind": "LinkedField",
              "name": "currency",
              "plural": false,
              "selections": (v7/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceRefundStatusDetails",
              "kind": "LinkedField",
              "name": "status",
              "plural": false,
              "selections": (v7/*:: as any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "requestedAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "lastProcessedAt",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "refundAmount",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "refundPercentage",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "currencyToDisplay",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "reason",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "lastError",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "externalRefundNumber",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "requestedByCustomerName",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "canProcessInXero",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "xeroProcessingBlockedReason",
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
      "concreteType": "RecurringBookingDetails",
      "kind": "LinkedField",
      "name": "recurringBooking",
      "plural": false,
      "selections": [
        (v2/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "startDate",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "endDate",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingFrequencyDetails",
          "kind": "LinkedField",
          "name": "frequency",
          "plural": false,
          "selections": [
            (v3/*:: as any*/)
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "MarketplaceBookingDetails",
          "kind": "LinkedField",
          "name": "marketplaceBooking",
          "plural": false,
          "selections": (v4/*:: as any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "storageKey": null
},
v9 = {
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
      "selections": (v7/*:: as any*/),
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "resourceCard_addPrivateBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addPrivateBooking",
        "plural": false,
        "selections": [
          (v8/*:: as any*/),
          (v9/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "resourceCard_addPrivateBookingMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "BookingPayload",
        "kind": "LinkedField",
        "name": "addPrivateBooking",
        "plural": false,
        "selections": [
          (v8/*:: as any*/),
          {
            "alias": null,
            "args": null,
            "filters": null,
            "handle": "appendNode",
            "key": "",
            "kind": "LinkedHandle",
            "name": "booking",
            "handleArgs": [
              {
                "kind": "Variable",
                "name": "connections",
                "variableName": "connectionIds"
              },
              {
                "kind": "Literal",
                "name": "edgeTypeName",
                "value": "BookingDetails"
              }
            ]
          },
          (v9/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "a8bf481275acccbcfded2af42995c814",
    "id": null,
    "metadata": {},
    "name": "resourceCard_addPrivateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation resourceCard_addPrivateBookingMutation(\n  $input: AddPrivateBookingInput!\n) {\n  addPrivateBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      notes\n      channel {\n        channel\n        name\n      }\n      category {\n        category\n        name\n      }\n      involvedCustomers {\n        id\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedOrganizations {\n        id\n      }\n      involvedLocations {\n        uniqueId\n        name\n      }\n      involvedTeams {\n        id\n        name\n      }\n      bookingResources {\n        resource {\n          id\n          name\n          color\n          customTags {\n            id\n            name\n            color\n          }\n          zones {\n            id\n            name\n            color\n          }\n        }\n      }\n      marketplaceBooking {\n        id\n        isPaymentRequired\n        paymentStatus {\n          type\n          name\n        }\n        invoiceUrl\n        refund {\n          id\n          currency {\n            type\n            name\n          }\n          status {\n            type\n            name\n          }\n          requestedAt\n          lastProcessedAt\n          refundAmount\n          refundPercentage\n          currencyToDisplay\n          reason\n          lastError\n          externalRefundNumber\n          requestedByCustomerName\n          canProcessInXero\n          xeroProcessingBlockedReason\n        }\n      }\n      recurringBooking {\n        id\n        startDate\n        endDate\n        frequency {\n          name\n        }\n        marketplaceBooking {\n          id\n        }\n      }\n    }\n    quotaError {\n      errorCode\n      reasonCode {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "49e896e6607a26ef3cb67e16dd9664db";

export default node;
