/**
 * @generated SignedSource<<13f30ad1b80379029918776c296ca9dd>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceMarketplaceBookingCategory_query$data = {
  readonly marketplaceBookingCategories: ReadonlyArray<{
    readonly category: BookingCategory;
    readonly name: string;
  }>;
  readonly " $fragmentType": "singleChoiceMarketplaceBookingCategory_query";
};
export type singleChoiceMarketplaceBookingCategory_query$key = {
  readonly " $data"?: singleChoiceMarketplaceBookingCategory_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMarketplaceBookingCategory_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceMarketplaceBookingCategory_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingCategoryDetails",
      "kind": "LinkedField",
      "name": "marketplaceBookingCategories",
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

(node as any).hash = "5e16a076bdfbb0cdf932313531b1536d";

export default node;
