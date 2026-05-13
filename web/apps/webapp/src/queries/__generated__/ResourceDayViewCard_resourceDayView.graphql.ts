/**
 * @generated SignedSource<<0ee51816801234c95fbf158d73de7ad8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ResourceAvailabilityClassification = "AVAILABLE" | "BLOCKED" | "FULLY_BOOKED" | "OCCUPIED" | "PARTIALLY_BOOKED" | "UNAVAILABLE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type ResourceDayViewCard_resourceDayView$data = {
  readonly bookedMinutes: number;
  readonly bookingWindows: ReadonlyArray<{
    readonly bookedByName: string | null | undefined;
    readonly bookingId: string;
    readonly from: any;
    readonly isCheckedIn: boolean;
    readonly isRecurring: boolean;
    readonly notes: string | null | undefined;
    readonly until: any;
  }>;
  readonly date: any;
  readonly floorId: string | null | undefined;
  readonly floorName: string | null | undefined;
  readonly locationId: string;
  readonly locationName: string;
  readonly openingFrom: any | null | undefined;
  readonly openingUntil: any | null | undefined;
  readonly resourceId: string;
  readonly resourceName: string;
  readonly resourceType: string;
  readonly status: ResourceAvailabilityClassification;
  readonly totalOpeningMinutes: number;
  readonly zoneId: string | null | undefined;
  readonly zoneName: string | null | undefined;
  readonly " $fragmentType": "ResourceDayViewCard_resourceDayView";
};
export type ResourceDayViewCard_resourceDayView$key = {
  readonly " $data"?: ResourceDayViewCard_resourceDayView$data;
  readonly " $fragmentSpreads": FragmentRefs<"ResourceDayViewCard_resourceDayView">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "ResourceDayViewCard_resourceDayView",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resourceId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resourceName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "resourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "locationId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "locationName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "floorId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "floorName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "zoneId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "zoneName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "date",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "status",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingFrom",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingUntil",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "totalOpeningMinutes",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "bookedMinutes",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingWindowDetails",
      "kind": "LinkedField",
      "name": "bookingWindows",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "bookingId",
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
          "name": "isRecurring",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isCheckedIn",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "bookedByName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "notes",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "ResourceDayViewDetails",
  "abstractKey": null
};

(node as any).hash = "b72b9e8014e04924e7e3f7a6d6cb7870";

export default node;
