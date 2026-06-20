/**
 * @generated SignedSource<<7a1edec5d27683e72af7e5cbc93c92a4>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type Currency = "NZD" | "USD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_booking_query$data = {
  readonly booking: {
    readonly arrearsInvoices: ReadonlyArray<{
      readonly billingPeriodEndExclusive: any;
      readonly billingPeriodStartInclusive: any;
      readonly invoiceNumber: string;
      readonly invoiceUrl: string;
    }>;
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
    };
    readonly from: any;
    readonly hasRecurringInstanceOverrides: boolean | null | undefined;
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
    }>;
    readonly involvedOrganizations: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
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
          readonly type: string;
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
  readonly " $fragmentType": "editMarketplaceBooking_booking_query";
};
export type editMarketplaceBooking_booking_query$key = {
  readonly " $data"?: editMarketplaceBooking_booking_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_booking_query">;
};

import editMarketplaceBooking_booking_refetchableFragment_graphql from './editMarketplaceBooking_booking_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
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
  (v0/*:: as any*/),
  (v1/*:: as any*/)
],
v3 = [
  (v1/*:: as any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v0/*:: as any*/),
  (v1/*:: as any*/),
  (v4/*:: as any*/)
],
v6 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*:: as any*/)
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "invoiceUrl",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": editMarketplaceBooking_booking_refetchableFragment_graphql
    }
  },
  "name": "editMarketplaceBooking_booking_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "bookingId"
        }
      ],
      "concreteType": "BookingDetails",
      "kind": "LinkedField",
      "name": "booking",
      "plural": false,
      "selections": [
        (v0/*:: as any*/),
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
          "kind": "ScalarField",
          "name": "hasRecurringInstanceOverrides",
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
            }
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
            (v0/*:: as any*/),
            (v1/*:: as any*/),
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
          "selections": (v2/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_LocationDetails",
          "kind": "LinkedField",
          "name": "involvedLocations",
          "plural": true,
          "selections": (v3/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamDetails",
          "kind": "LinkedField",
          "name": "involvedTeams",
          "plural": true,
          "selections": (v2/*:: as any*/),
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
                (v0/*:: as any*/),
                (v1/*:: as any*/),
                (v4/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "customTags",
                  "plural": true,
                  "selections": (v5/*:: as any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v5/*:: as any*/),
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
            (v0/*:: as any*/),
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
              "selections": (v6/*:: as any*/),
              "storageKey": null
            },
            (v7/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "MarketplaceRefundDetails",
              "kind": "LinkedField",
              "name": "refund",
              "plural": false,
              "selections": [
                (v0/*:: as any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CurrencyDetails",
                  "kind": "LinkedField",
                  "name": "currency",
                  "plural": false,
                  "selections": (v6/*:: as any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "MarketplaceRefundStatusDetails",
                  "kind": "LinkedField",
                  "name": "status",
                  "plural": false,
                  "selections": (v6/*:: as any*/),
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
            (v0/*:: as any*/),
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
              "selections": (v3/*:: as any*/),
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
                (v0/*:: as any*/)
              ],
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationArrearsInvoiceDetails",
          "kind": "LinkedField",
          "name": "arrearsInvoices",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "invoiceNumber",
              "storageKey": null
            },
            (v7/*:: as any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "billingPeriodStartInclusive",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "billingPeriodEndExclusive",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0ff3f8f5aa0c84acdcd3bb04878dbbed";

export default node;
