/**
 * @generated SignedSource<<dc3d256022e712211c894574b31c9bab>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "HOST" | "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocationCard_LocationDetails$data = {
  readonly extraMetadata: {
    readonly areaRange: {
      readonly fromInSqm: string;
      readonly toInSqm: string;
    } | null | undefined;
    readonly peopleCapacity: {
      readonly from: string;
      readonly to: string;
    } | null | undefined;
  } | null | undefined;
  readonly featureImages: ReadonlyArray<{
    readonly thumbnail: {
      readonly height: number | null | undefined;
      readonly url: string;
      readonly width: number | null | undefined;
    } | null | undefined;
  }>;
  readonly id: string;
  readonly name: string;
  readonly organization: {
    readonly spacesPublicBookingAvailability: {
      readonly available: boolean;
      readonly message: string;
    };
    readonly type: {
      readonly name: string;
      readonly type: OrganizationType;
    };
  };
  readonly physicalAddress: {
    readonly multilinesFormattedAddress: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "marketplaceLocationCard_LocationDetails";
};
export type marketplaceLocationCard_LocationDetails$key = {
  readonly " $data"?: marketplaceLocationCard_LocationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocationCard_LocationDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "marketplaceLocationCard_LocationDetails",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "id",
      "storageKey": null
    },
    (v0/*:: as any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTypeDetails",
          "kind": "LinkedField",
          "name": "type",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "type",
              "storageKey": null
            },
            (v0/*:: as any*/)
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "SpacesPublicBookingAvailabilityDetails",
          "kind": "LinkedField",
          "name": "spacesPublicBookingAvailability",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "available",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "message",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationExtraMetadata",
      "kind": "LinkedField",
      "name": "extraMetadata",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "AreaRange",
          "kind": "LinkedField",
          "name": "areaRange",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "fromInSqm",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "toInSqm",
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "PeopleCapacity",
          "kind": "LinkedField",
          "name": "peopleCapacity",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "from",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "to",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationPhysicalAddressDetails",
      "kind": "LinkedField",
      "name": "physicalAddress",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "multilinesFormattedAddress",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CdnImageFile",
      "kind": "LinkedField",
      "name": "featureImages",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnFile",
          "kind": "LinkedField",
          "name": "thumbnail",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "url",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "height",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "width",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "LocationDetails",
  "abstractKey": null
};
})();

(node as any).hash = "7ee4c27382d854a1bb9438f9fceeeca5";

export default node;
