/**
 * @generated SignedSource<<d49f0b8ae0c5504f6765d234887eeb2b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type LocationRestrictedInformationCategory = "ACCESS" | "ACCESSIBILITY" | "AFTER_HOURS" | "CHECK_IN" | "CHECK_OUT" | "CLEANING" | "DELIVERIES" | "EQUIPMENT" | "EVACUATION" | "GUESTS" | "HOUSE_RULES" | "KITCHEN" | "MAINTENANCE" | "MEETING_ROOMS" | "NOISE" | "OTHER" | "PARKING" | "PETS" | "SECURITY" | "SMOKING" | "STORAGE" | "WASTE" | "WIFI" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceLocationRestrictedInformationCategory_query$data = {
  readonly locationRestrictedInformationCategories: ReadonlyArray<{
    readonly category: LocationRestrictedInformationCategory;
    readonly name: string;
  }>;
  readonly " $fragmentType": "singleChoiceLocationRestrictedInformationCategory_query";
};
export type singleChoiceLocationRestrictedInformationCategory_query$key = {
  readonly " $data"?: singleChoiceLocationRestrictedInformationCategory_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceLocationRestrictedInformationCategory_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceLocationRestrictedInformationCategory_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationRestrictedInformationCategoryDetails",
      "kind": "LinkedField",
      "name": "locationRestrictedInformationCategories",
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

(node as any).hash = "f436b142b422b9159e79f007207041ef";

export default node;
