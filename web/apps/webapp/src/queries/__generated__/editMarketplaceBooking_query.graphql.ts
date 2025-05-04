/**
 * @generated SignedSource<<7c5a03a950583aa63d455f79176ce03a>>
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
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMarketplaceBookingType_query">;
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
      "name": "singleChoiceMarketplaceBookingType_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "08cd1384ec99d1d3c913a6cb4c59d633";

export default node;
