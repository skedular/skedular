/**
 * @generated SignedSource<<627ca07574af8aba30f0a9d394c380c2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "AnnualLeave" | "ClientOffice" | "NonWorkingDay" | "SickLeave" | "TravelingForWork" | "Vacation" | "WellbeingLeave" | "WorkingFromHome" | "WorkingFromOffice" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceBookingType_query$data = {
  readonly bookingTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: BookingType;
  }>;
  readonly " $fragmentType": "singleChoiceBookingType_query";
};
export type singleChoiceBookingType_query$key = {
  readonly " $data"?: singleChoiceBookingType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceBookingType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceBookingType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTypeDetails",
      "kind": "LinkedField",
      "name": "bookingTypes",
      "plural": true,
      "selections": [
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
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "ee9d302412f92be233f4a65b2ec4bc2f";

export default node;
