/**
 * @generated SignedSource<<9a31bb3759aa1246dbb4b3537c11c721>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationTagType = "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type resourceTypeSelector_allResourceTypes_query$data = {
  readonly resourceTypes: ReadonlyArray<{
    readonly name: string;
    readonly tagType: OrganizationTagType;
  }>;
  readonly " $fragmentType": "resourceTypeSelector_allResourceTypes_query";
};
export type resourceTypeSelector_allResourceTypes_query$key = {
  readonly " $data"?: resourceTypeSelector_allResourceTypes_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"resourceTypeSelector_allResourceTypes_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "resourceTypeSelector_allResourceTypes_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagTypeDetails",
      "kind": "LinkedField",
      "name": "resourceTypes",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "tagType",
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

(node as any).hash = "308e7ed2a368453828eac50e91a97295";

export default node;
