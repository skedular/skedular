/**
 * @generated SignedSource<<d04b782401c1ecc52ec0cdadab603222>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editMarketplaceBooking_query$data = {
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
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceMarketplaceBookingCategory_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "44794e14a4aadbb8c331a5d16b0b3308";

export default node;
