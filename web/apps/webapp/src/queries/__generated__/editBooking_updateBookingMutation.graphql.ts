/**
 * @generated SignedSource<<b0f34c193c38cab49113bed3848b4e47>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type BookingType = "AnnualLeave" | "ClientOffice" | "NonWorkingDay" | "SickLeave" | "TravelingForWork" | "Vacation" | "WellbeingLeave" | "WorkingFromCoworkingSpace" | "WorkingFromHome" | "WorkingFromOffice" | "%future added value";
export type UpdateBookingInput = {
  clientMutationId?: string | null | undefined;
  customerIds: ReadonlyArray<string>;
  from: any;
  id: string;
  notes?: string | null | undefined;
  organizationIds: ReadonlyArray<string>;
  resourceIds: ReadonlyArray<string>;
  teamIds: ReadonlyArray<string>;
  type: BookingType;
  until: any;
};
export type editBooking_updateBookingMutation$variables = {
  input: UpdateBookingInput;
};
export type editBooking_updateBookingMutation$data = {
  readonly updateBooking: {
    readonly booking: {
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
        readonly name: string;
        readonly type: BookingType;
      };
      readonly until: any;
    };
  } | null | undefined;
};
export type editBooking_updateBookingMutation$rawResponse = {
  readonly updateBooking: {
    readonly booking: {
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
        readonly name: string;
        readonly type: BookingType;
      };
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
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v3 = [
  (v2/*: any*/),
  (v1/*: any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v2/*: any*/),
  (v1/*: any*/),
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
              },
              (v1/*: any*/)
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
              (v2/*: any*/),
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
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "involvedLocations",
            "plural": true,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_TeamDetails",
            "kind": "LinkedField",
            "name": "involvedTeams",
            "plural": true,
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
              (v2/*: any*/),
              (v1/*: any*/),
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
    "cacheID": "e109abc0fa055058c5a8c03cbe7aa746",
    "id": null,
    "metadata": {},
    "name": "editBooking_updateBookingMutation",
    "operationKind": "mutation",
    "text": "mutation editBooking_updateBookingMutation(\n  $input: UpdateBookingInput!\n) {\n  updateBooking(input: $input) {\n    booking {\n      id\n      from\n      until\n      notes\n      type {\n        type\n        name\n      }\n      involvedCustomers {\n        uniqueId\n        name\n        givenName\n        middleName\n        familyName\n        photoUrl\n      }\n      involvedOrganizations {\n        uniqueId\n        name\n      }\n      involvedLocations {\n        uniqueId\n        name\n      }\n      involvedTeams {\n        uniqueId\n        name\n      }\n      resources {\n        uniqueId\n        name\n        color\n        customTags {\n          uniqueId\n          name\n          color\n        }\n        zones {\n          uniqueId\n          name\n          color\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b2d550c1efc13b9ffd339089b306c508";

export default node;
