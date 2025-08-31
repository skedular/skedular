/**
 * @generated SignedSource<<a884ccf07f66039c1c8228207371750d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type marketplaceLocationCard_LocationDetails$data = {
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

(node as any).hash = "e6071eaec8c8fa751f8c04d9ea22fe4c";

export default node;
