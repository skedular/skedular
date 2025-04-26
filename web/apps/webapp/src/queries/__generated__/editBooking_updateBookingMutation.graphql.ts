/**
 * @generated SignedSource<<b083a2f0bfb531bf65f14c4895e2f773>>
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
  from: any;
  id: string;
  locationId?: string | null | undefined;
  notes?: string | null | undefined;
  organizationId?: string | null | undefined;
  productVersionIds: ReadonlyArray<string>;
  resourceIds: ReadonlyArray<string>;
  teamId?: string | null | undefined;
  type: BookingType;
  until: any;
};
export type editBooking_updateBookingMutation$variables = {
  input: UpdateBookingInput;
};
export type editBooking_updateBookingMutation$data = {
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
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly type: BookingType;
      readonly until: any;
    };
  } | null | undefined;
};
export type editBooking_updateBookingMutation$rawResponse = {
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
      readonly team: {
        readonly name: string;
        readonly uniqueId: string;
      } | null | undefined;
      readonly type: BookingType;
      readonly until: any;
    };
  } | null | undefined;
};
export type editBooking_updateBookingMutation = {
  rawResponse: editBooking_updateBookingMutation$rawResponse;
  response: editBooking_updateBookingMutation$data;
  variables: editBooking_updateBookingMutation$variables;
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
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v1/*: any*/),
  (v2/*: any*/),
  (v4/*: any*/)
],
v6 = [
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
            "name": "type",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_CustomerDetails",
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
            "concreteType": "Booking_OrganizationDetails",
            "kind": "LinkedField",
            "name": "organization",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_TeamDetails",
            "kind": "LinkedField",
            "name": "team",
            "plural": false,
            "selections": (v3/*: any*/),
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
              (v1/*: any*/),
              (v2/*: any*/),
              (v4/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_OrganizationCustomTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v5/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "Booking_OrganizationZoneDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v5/*: any*/),
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
    "name": "editBooking_updateBookingMutation",
    "selections": (v6/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editBooking_updateBookingMutation",
    "selections": (v6/*: any*/)
  },
  "params": {
    "cacheID": "bc6b487a2321b9896b441307a8298aa5",
    "id": null,
    "metadata": {},
    "name": "editBooking_updateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editBooking_updateBookingMutation(\n  $input: UpdateBookingInput!\n) {\n  updateBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      notes\n      type\n      customer {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      organization {\n        uniqueId\n        name\n      }\n      location {\n        uniqueId\n        name\n      }\n      team {\n        uniqueId\n        name\n      }\n      resources {\n        uniqueId\n        name\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "1a1c753512e68c64de9c5cec4398c141";

export default node;
