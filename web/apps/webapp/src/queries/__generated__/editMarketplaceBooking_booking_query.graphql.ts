/**
 * @generated SignedSource<<500a0517c37cc5f93c9924857628d686>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type PaymentStatus = "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PAID" | "PENDING" | "UNPAID" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_booking_query$data = {
  readonly booking: {
    readonly bookingCheckoutSession: {
      readonly amountTotalToDisplay: string;
      readonly checkoutUrl: string;
      readonly paymentStatus: PaymentStatus;
    } | null | undefined;
    readonly bookingCheckoutSessionExpiry: any;
    readonly from: any;
    readonly id: string;
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
    readonly notes: string | null | undefined;
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
    readonly type: {
      readonly type: BookingType;
    };
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
  "name": "uniqueId",
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
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  (v0/*: any*/),
  (v1/*: any*/),
  (v3/*: any*/)
];
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
          "selections": [
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
          "concreteType": "Booking_CustomerDetails",
          "kind": "LinkedField",
          "name": "involvedCustomers",
          "plural": true,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
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
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_LocationDetails",
          "kind": "LinkedField",
          "name": "involvedLocations",
          "plural": true,
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_TeamDetails",
          "kind": "LinkedField",
          "name": "involvedTeams",
          "plural": true,
          "selections": (v2/*: any*/),
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
            (v0/*: any*/),
            (v1/*: any*/),
            (v3/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_OrganizationCustomTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v4/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_OrganizationZoneDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v4/*: any*/),
              "storageKey": null
            }
          ],
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
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "paymentStatus",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "amountTotalToDisplay",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "bookingCheckoutSessionExpiry",
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

(node as any).hash = "5ee4217b1d9c786e8a8afe810e5a7903";

export default node;
