/**
 * @generated SignedSource<<46d5287ccf945ff29ac4a9e5901ca669>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myLocationCard_LocationDetails$data = {
  readonly deskTypes: ReadonlyArray<{
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly desks: ReadonlyArray<{
    readonly id: string;
  }>;
  readonly id: string;
  readonly name: string;
  readonly physicalAddress: {
    readonly formattedAddress: string | null | undefined;
  } | null | undefined;
  readonly zones: ReadonlyArray<{
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly " $fragmentType": "myLocationCard_LocationDetails";
};
export type myLocationCard_LocationDetails$key = {
  readonly " $data"?: myLocationCard_LocationDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"myLocationCard_LocationDetails">;
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myLocationCard_LocationDetails",
  "selections": [
    (v0/*: any*/),
    (v1/*: any*/),
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "deskTypes",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v2/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "DeskDetails",
      "kind": "LinkedField",
      "name": "desks",
      "plural": true,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "LocationAddressDetails",
      "kind": "LinkedField",
      "name": "physicalAddress",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "formattedAddress",
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

(node as any).hash = "d05d016920b93bd0cf4949ec5bcbdaba";

export default node;
