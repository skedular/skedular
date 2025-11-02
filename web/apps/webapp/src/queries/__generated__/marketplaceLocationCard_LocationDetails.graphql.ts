/**
 * @generated SignedSource<<8d4d2e96cb2f902bd82152e2cacc9758>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
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
  readonly id: string;
  readonly name: string;
  readonly physicalAddress: {
    readonly multilinesFormattedAddress: string | null | undefined;
  } | null | undefined;
  readonly primaryFeatureImage: {
    readonly thumbnail: {
      readonly height: number | null | undefined;
      readonly url: string;
      readonly width: number | null | undefined;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "marketplaceLocationCard_LocationDetails";
};
export type marketplaceLocationCard_LocationDetails$key = {
  readonly " $data"?: marketplaceLocationCard_LocationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"marketplaceLocationCard_LocationDetails">;
};

const node: ReaderFragment = {
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
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "name",
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
      "name": "primaryFeatureImage",
      "plural": false,
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

(node as any).hash = "25cdadf8794c121c0da432b348164454";

export default node;
