/**
 * @generated SignedSource<<97014feb11fec0f6468734f4fb150ae0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationTagType = "CUSTOM" | "LOCATION_SPACE_TYPE_CAR_PARK_SPACE" | "LOCATION_SPACE_TYPE_COMMERCIAL_KITCHEN" | "LOCATION_SPACE_TYPE_EVENT_SPACE" | "LOCATION_SPACE_TYPE_MEETING_SPACE" | "LOCATION_SPACE_TYPE_OFFICE_SPACE" | "LOCATION_SPACE_TYPE_OTHERS" | "LOCATION_SPACE_TYPE_RETAIL_SPACE" | "LOCATION_SPACE_TYPE_SHOOT_LOCATION" | "LOCATION_SPACE_TYPE_STORAGE_SPACE" | "LOCATION_SPACE_TYPE_STUDIO_SPACE" | "PRODUCT" | "RESOURCE_DESK" | "RESOURCE_OTHERS" | "RESOURCE_PARKING" | "RESOURCE_ROOM" | "ZONE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type resourceCard_ResourceDetails$data = {
  readonly capacity: number;
  readonly color: string | null | undefined;
  readonly customTags: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
  readonly id: string;
  readonly inactive: boolean;
  readonly name: string;
  readonly productTags: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
  readonly resourceType: {
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly tagType: OrganizationTagType | null | undefined;
  };
  readonly zones: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
  readonly " $fragmentType": "resourceCard_ResourceDetails";
};
export type resourceCard_ResourceDetails$key = {
  readonly " $data"?: resourceCard_ResourceDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"resourceCard_ResourceDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v3 = [
  (v0/*: any*/),
  (v1/*: any*/),
  (v2/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "resourceCard_ResourceDetails",
  "selections": [
    (v0/*: any*/),
    (v1/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
    (v2/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "capacity",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "customTags",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "productTags",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "resourceType",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "tagType",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "ResourceDetails",
  "abstractKey": null
};
})();

(node as any).hash = "4f69d93a39b385547c17f6fff90a7634";

export default node;
