/**
 * @generated SignedSource<<5c3263487205c0241b98fd82227ecee8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceBookingCategory_query$data = {
  readonly bookingCategories: ReadonlyArray<{
    readonly category: BookingCategory;
    readonly name: string;
  }>;
  readonly " $fragmentType": "singleChoiceBookingCategory_query";
};
export type singleChoiceBookingCategory_query$key = {
  readonly " $data"?: singleChoiceBookingCategory_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceBookingCategory_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceBookingCategory_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingCategoryDetails",
      "kind": "LinkedField",
      "name": "bookingCategories",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "category",
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

(node as any).hash = "e50a973f456d3a98048ce57c75080c59";

export default node;
