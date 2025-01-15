/**
 * @generated SignedSource<<bd36c0662f2e033e9e96f402c3611c94>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "AnnualLeave" | "ClientOffices" | "NonWorkingDay" | "SickLeave" | "TravelingForWork" | "Vacation" | "WellBeingLeave" | "WorkingFromHome" | "WorkingFromOffice" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookingCard_BookingDetails$data = {
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
v3 = [
  (v0/*: any*/),
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
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
      "concreteType": "BookingOrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingLocationDetails",
      "kind": "LinkedField",
      "name": "location",
      "plural": false,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTeamDetails",
      "kind": "LinkedField",
      "name": "team",
      "plural": false,
      "selections": (v2/*: any*/),
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
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingOrganizationCustomTagDetails",
          "kind": "LinkedField",
          "name": "customTags",
          "plural": true,
          "selections": (v3/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingOrganizationZoneDetails",
          "kind": "LinkedField",
          "name": "zones",
          "plural": true,
          "selections": (v3/*: any*/),
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

(node as any).hash = "23d296a650ccd13c6c28b0693717317a";

export default node;
