/**
 * @generated SignedSource<<838209c4190e800bc0e85a1ea3ba3e4c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingType = "AnnualLeave" | "ClientOffices" | "NonWorkingDay" | "SickLeave" | "TravelingForWork" | "Vacation" | "WellBeingLeave" | "WorkingFromHome" | "WorkingFromOffice" | "%future added value";
export type UpdateBookingInput = {
  clientMutationId?: string | null | undefined;
  customerId: string;
  deskIds: ReadonlyArray<string>;
  from: any;
  id: string;
  locationId?: string | null | undefined;
  notes?: string | null | undefined;
  organizationId?: string | null | undefined;
  teamId?: string | null | undefined;
  to: any;
  type: BookingType;
};
export type bookingCard_updateBookingMutation$variables = {
  input: UpdateBookingInput;
};
export type bookingCard_updateBookingMutation$data = {
  readonly updateBooking: {
    readonly booking: {
      readonly customer: {
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly desks: ReadonlyArray<{
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
      readonly from: any;
      readonly id: string;
      readonly location: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly organization: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly to: any;
      readonly type: BookingType;
    };
  } | null | undefined;
};
export type bookingCard_updateBookingMutation$rawResponse = {
  readonly updateBooking: {
    readonly booking: {
      readonly customer: {
        readonly familyName: string | null | undefined;
        readonly givenName: string | null | undefined;
        readonly middleName: string | null | undefined;
        readonly name: string | null | undefined;
        readonly photoUrl: string | null | undefined;
        readonly uniqueId: string;
      };
      readonly desks: ReadonlyArray<{
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
      readonly from: any;
      readonly id: string;
      readonly location: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly notes: string | null | undefined;
      readonly organization: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly to: any;
      readonly type: BookingType;
    };
  } | null | undefined;
};
export type bookingCard_updateBookingMutation = {
  rawResponse: bookingCard_updateBookingMutation$rawResponse;
  response: bookingCard_updateBookingMutation$data;
  variables: bookingCard_updateBookingMutation$variables;
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
  "name": "uniqueId",
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
  (v1/*: any*/),
  (v2/*: any*/)
],
v4 = [
  (v1/*: any*/),
  (v2/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
],
v5 = [
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
    "name": "updateBooking",
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
            "name": "to",
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
            "name": "type",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingCustomerDetails",
            "kind": "LinkedField",
            "name": "customer",
            "plural": false,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/),
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
            "concreteType": "BookingOrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingLocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingTeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "desks",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingOrganizationCustomTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v4/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingOrganizationZoneDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v4/*: any*/),
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookingCard_updateBookingMutation",
    "selections": (v5/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingCard_updateBookingMutation",
    "selections": (v5/*: any*/)
  },
  "params": {
    "cacheID": "6a07853036e209290432e4d178833f02",
    "id": null,
    "metadata": {},
    "name": "bookingCard_updateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation bookingCard_updateBookingMutation(\n  $input: UpdateBookingInput!\n) {\n  updateBooking(input: $input) {\n    booking {\n      id\n      from\n      to\n      notes\n      type\n      customer {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      organization {\n        uniqueId\n        name\n      }\n      location {\n        uniqueId\n        name\n      }\n      team {\n        uniqueId\n        name\n      }\n      desks {\n        uniqueId\n        name\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "876494161c89ca72b68996b3a19857fb";

export default node;
