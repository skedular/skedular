/**
 * @generated SignedSource<<364329c7b3ce5f2e655d56f5e393e988>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type payMarketplaceBooking_booking_query$data = {
  readonly booking: {
    readonly bookingCheckoutSession: {
      readonly checkoutUrl: string;
    } | null | undefined;
    readonly from: any;
    readonly id: string;
    readonly invoiceUrl: string | null | undefined;
    readonly involvedCustomers: ReadonlyArray<{
      readonly familyName: string | null | undefined;
      readonly givenName: string | null | undefined;
      readonly middleName: string | null | undefined;
      readonly name: string | null | undefined;
      readonly photoUrl: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly involvedLocations: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
    readonly involvedOrganizations: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
    readonly involvedTeams: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
    readonly isPaymentRequired: boolean;
    readonly lineItems: ReadonlyArray<{
      readonly productVersion: {
        readonly name: string;
        readonly priceToDisplay: string;
        readonly uniqueId: string;
      };
      readonly quantity: number;
    }>;
    readonly notes: string | null | undefined;
    readonly paymentExpiry: any;
    readonly paymentMethod: {
      readonly type: PaymentMethod;
    } | null | undefined;
    readonly paymentStatus: {
      readonly name: string;
      readonly type: PaymentStatus;
    };
    readonly resources: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly customTags: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
      readonly name: string;
      readonly uniqueId: string;
      readonly zones: ReadonlyArray<{
        readonly color: string | null | undefined;
        readonly name: string | null | undefined;
        readonly uniqueId: string;
      }>;
    }>;
    readonly taxAmountToDisplay: string;
    readonly totalAmountExcludeTaxToDisplay: string;
    readonly totalAmountToDisplay: string;
    readonly type: {
      readonly type: BookingType;
    };
    readonly until: any;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canModifyPaymentMethod: boolean;
  };
  readonly paymentStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentStatus;
  }>;
  readonly " $fragmentType": "payMarketplaceBooking_booking_query";
};
export type payMarketplaceBooking_booking_query$key = {
  readonly " $data"?: payMarketplaceBooking_booking_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"payMarketplaceBooking_booking_query">;
};

import payMarketplaceBooking_booking_refetchableFragment_graphql from './payMarketplaceBooking_booking_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "type",
  "storageKey": null
},
v1 = [
  (v0/*: any*/)
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
  (v2/*: any*/),
  (v3/*: any*/)
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v6 = [
  (v2/*: any*/),
  (v3/*: any*/),
  (v5/*: any*/)
],
v7 = [
  (v0/*: any*/),
  (v3/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": payMarketplaceBooking_booking_refetchableFragment_graphql
    }
  },
  "name": "payMarketplaceBooking_booking_query",
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
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
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
          "name": "notes",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingTypeDetails",
          "kind": "LinkedField",
          "name": "type",
          "plural": false,
          "selections": (v1/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_CustomerDetails",
          "kind": "LinkedField",
          "name": "involvedCustomers",
          "plural": true,
          "selections": [
            (v2/*: any*/),
            (v3/*: any*/),
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
          "concreteType": "Booking_OrganizationDetails",
          "kind": "LinkedField",
          "name": "involvedOrganizations",
          "plural": true,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_LocationDetails",
          "kind": "LinkedField",
          "name": "involvedLocations",
          "plural": true,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_TeamDetails",
          "kind": "LinkedField",
          "name": "involvedTeams",
          "plural": true,
          "selections": (v4/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingResourceDetails",
          "kind": "LinkedField",
          "name": "resources",
          "plural": true,
          "selections": [
            (v2/*: any*/),
            (v3/*: any*/),
            (v5/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_OrganizationCustomTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v6/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_OrganizationZoneDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v6/*: any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalAmountExcludeTaxToDisplay",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "taxAmountToDisplay",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalAmountToDisplay",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PaymentMethodTypeDetails",
          "kind": "LinkedField",
          "name": "paymentMethod",
          "plural": false,
          "selections": (v1/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingCheckoutSessionDetails",
          "kind": "LinkedField",
          "name": "bookingCheckoutSession",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "checkoutUrl",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "paymentExpiry",
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
          "concreteType": "LineItemDetails",
          "kind": "LinkedField",
          "name": "lineItems",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "quantity",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_ProductVersionDetails",
              "kind": "LinkedField",
              "name": "productVersion",
              "plural": false,
              "selections": [
                (v2/*: any*/),
                (v3/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "priceToDisplay",
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
          "selections": (v7/*: any*/),
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
          "name": "organizationUniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationBookingPermissions",
      "kind": "LinkedField",
      "name": "organizationBookingPermissions",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canModifyPaymentMethod",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentStatusDetails",
      "kind": "LinkedField",
      "name": "paymentStatuses",
      "plural": true,
      "selections": (v7/*: any*/),
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "b0a0198a6ba13eb4852f55ae5549ccd2";

export default node;
