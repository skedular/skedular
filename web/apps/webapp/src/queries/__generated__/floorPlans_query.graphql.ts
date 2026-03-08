/**
 * @generated SignedSource<<505b49b8c67361b60d2a3c34fc2fd520>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationTagType = "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type floorPlans_query$data = {
  readonly deskResourceType: OrganizationTagType;
  readonly me: {
    readonly id: string;
  };
  readonly parkingResourceType: OrganizationTagType;
  readonly roomResourceType: OrganizationTagType;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query" | "customTagSelector_allCustomTags_query" | "floorPlanSelector_allFloorPlans_query" | "organizationUserSelector_organizationMembers_query" | "resourceCard_query" | "zoneSelector_allZones_query">;
  readonly " $fragmentType": "floorPlans_query";
};
export type floorPlans_query$key = {
  readonly " $data"?: floorPlans_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlans_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "floorPlans_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "deskResourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "roomResourceType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "parkingResourceType",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "customTagSelector_allCustomTags_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneSelector_allZones_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "floorPlanSelector_allFloorPlans_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationUserSelector_organizationMembers_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingCard_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "resourceCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "14a87520e3dae69df48ae7e378bb8bf1";

export default node;
