/**
 * @generated SignedSource<<65190773878ced5ed299f3dd70a85be2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationCard_LocationDetails$data = {
  readonly canDelete: boolean;
  readonly canModify: boolean;
  readonly contactedViaCall: boolean;
  readonly contactedViaEmail: boolean;
  readonly contactedViaSms: boolean;
  readonly contactedViaWhatsapp: boolean;
  readonly customTags: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
  readonly extraMetadata: {
    readonly contactDetails: {
      readonly contactEmails: ReadonlyArray<string> | null | undefined;
      readonly contactPeople: ReadonlyArray<string> | null | undefined;
      readonly contactPhones: ReadonlyArray<string> | null | undefined;
    } | null | undefined;
  } | null | undefined;
  readonly featureImages: ReadonlyArray<{
    readonly thumbnail: {
      readonly height: number | null | undefined;
      readonly url: string;
      readonly width: number | null | undefined;
    } | null | undefined;
  }>;
  readonly hasFutureBooking: boolean;
  readonly id: string;
  readonly name: string;
  readonly organization: {
    readonly uniqueAlphanumericName: string | null | undefined;
  };
  readonly physicalAddress: {
    readonly latitude: number | null | undefined;
    readonly longitude: number | null | undefined;
    readonly multilinesFormattedAddress: string | null | undefined;
  } | null | undefined;
  readonly resources: {
    readonly totalCount: number;
  };
  readonly uniqueClaimCode: string | null | undefined;
  readonly zones: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly id: string;
    readonly name: string;
  }>;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationCard_LocationDetails",
  "selections": [
    (v0/*: any*/),
    (v1/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "customTags",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "ConnectionOfResourceEdge",
      "kind": "LinkedField",
      "name": "resources",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "totalCount",
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
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "latitude",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "longitude",
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "hasFutureBooking",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "canModify",
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
          "name": "uniqueAlphanumericName",
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
          "concreteType": "ContactDetails",
          "kind": "LinkedField",
          "name": "contactDetails",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "contactPeople",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "contactEmails",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "contactPhones",
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
      "kind": "ScalarField",
      "name": "uniqueClaimCode",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "contactedViaEmail",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "contactedViaCall",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "contactedViaSms",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "contactedViaWhatsapp",
      "storageKey": null
    }
  ],
  "type": "LocationDetails",
  "abstractKey": null
};
})();

(node as any).hash = "13f280d784fb3a6b19212baceb0a5cdd";

export default node;
