/**
 * @generated SignedSource<<f92afc914b37961e0ab9f7025c2e28c9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingType = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceMarketplaceBookingType_query$data = {
  readonly marketplaceBookingTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: BookingType;
  }>;
  readonly " $fragmentType": "singleChoiceMarketplaceBookingType_query";
};
export type singleChoiceMarketplaceBookingType_query$key = {
  readonly " $data"?: singleChoiceMarketplaceBookingType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMarketplaceBookingType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceMarketplaceBookingType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "BookingTypeDetails",
      "kind": "LinkedField",
      "name": "marketplaceBookingTypes",
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

(node as any).hash = "d3d74291d563bf16a835eb2147f1020a";

export default node;
