/**
 * @generated SignedSource<<aa9e9b2349919a266c362e8066f8457d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_query$data = {
  readonly openingHoursMinutesStep: number;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMarketplaceBookingCategory_query">;
  readonly " $fragmentType": "editMarketplaceBooking_query";
};
export type editMarketplaceBooking_query$key = {
  readonly " $data"?: editMarketplaceBooking_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editMarketplaceBooking_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "editMarketplaceBooking_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "openingHoursMinutesStep",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceMarketplaceBookingCategory_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "7dfb467d01f5857af3c390f73c3666b3";

export default node;
