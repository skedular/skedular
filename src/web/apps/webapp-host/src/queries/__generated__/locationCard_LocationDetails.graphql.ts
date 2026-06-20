/**
 * @generated SignedSource<<e127a96e7711fc2365afbb215466cfe1>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationCard_LocationDetails$data = {
  readonly canDelete: boolean;
  readonly featureImages: ReadonlyArray<{
    readonly original: {
      readonly url: string;
    } | null | undefined;
    readonly thumbnail: {
      readonly url: string;
    } | null | undefined;
  }>;
  readonly id: string;
  readonly name: string;
  readonly organization: {
    readonly customDomain: string | null | undefined;
  };
  readonly physicalAddress: {
    readonly multilinesFormattedAddress: string | null | undefined;
  } | null | undefined;
  readonly products: ReadonlyArray<{
    readonly id: string;
  }>;
  readonly uniqueClaimCode: string | null | undefined;
  readonly " $fragmentType": "locationCard_LocationDetails";
};
export type locationCard_LocationDetails$key = {
  readonly " $data"?: locationCard_LocationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationCard_LocationDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationCard_LocationDetails",
  "selections": [
    (v0/*:: as any*/),
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
      "concreteType": "ProductDetails",
      "kind": "LinkedField",
      "name": "products",
      "plural": true,
      "selections": [
        (v0/*:: as any*/)
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
          "name": "original",
          "plural": false,
          "selections": (v1/*:: as any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "CdnFile",
          "kind": "LinkedField",
          "name": "thumbnail",
          "plural": false,
          "selections": (v1/*:: as any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canDelete",
      "storageKey": null
    },
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
          "kind": "ScalarField",
          "name": "customDomain",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "uniqueClaimCode",
      "storageKey": null
    }
  ],
  "type": "LocationDetails",
  "abstractKey": null
};
})();

(node as any).hash = "a899cd585fdbdb9590347a1c0569fd5b";

export default node;
