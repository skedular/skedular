/**
 * @generated SignedSource<<52c16891a8fdf798c768d866d8c5eb82>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "AnnualLeave" | "ClientOffice" | "NonWorkingDay" | "SickLeave" | "TravelingForWork" | "Vacation" | "WellbeingLeave" | "WorkingFromCoworkingSpace" | "WorkingFromHome" | "WorkingFromOffice" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookingCard_BookingDetails$data = {
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
  readonly " $fragmentType": "bookingCard_BookingDetails";
};
export type bookingCard_BookingDetails$key = {
  readonly " $data"?: bookingCard_BookingDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v2 = [
  (v1/*: any*/),
  (v0/*: any*/)
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  (v1/*: any*/),
  (v0/*: any*/),
  (v3/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingCard_BookingDetails",
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
        (v0/*: any*/)
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
        (v1/*: any*/),
        (v0/*: any*/),
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
      "selections": [
        (v1/*: any*/)
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
        (v1/*: any*/),
        (v0/*: any*/),
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
    }
  ],
  "type": "BookingDetails",
  "abstractKey": null
};
})();

(node as any).hash = "40f729d43c1b0e4ccdbf97fa35d7116d";

export default node;
