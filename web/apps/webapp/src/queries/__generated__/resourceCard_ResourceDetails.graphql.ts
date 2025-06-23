/**
 * @generated SignedSource<<6d5622cc1ba88cc03fb4f2c412953e2b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type resourceCard_ResourceDetails$data = {
  readonly capacity: number;
  readonly color: string | null | undefined;
  readonly customTags: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly id: string;
  readonly inactive: boolean;
  readonly name: string;
  readonly productTags: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly resourceType: {
    readonly color: string | null | undefined;
    readonly name: string | null | undefined;
    readonly tagType: string | null | undefined;
    readonly uniqueId: string;
  };
  readonly zones: ReadonlyArray<{
    readonly color: string | null | undefined;
    readonly name: string | null | undefined;
    readonly uniqueId: string;
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
  "name": "name",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v3 = [
  (v2/*: any*/),
  (v0/*: any*/),
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "resourceCard_ResourceDetails",
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "id",
      "storageKey": null
    },
    (v0/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "inactive",
      "storageKey": null
    },
    (v1/*: any*/),
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
      "concreteType": "Location_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "customTags",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Location_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Location_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "productTags",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Location_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "resourceType",
      "plural": false,
      "selections": [
        (v2/*: any*/),
        (v0/*: any*/),
        (v1/*: any*/),
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

(node as any).hash = "3b9cb704dc9db66b387447aa549b30d0";

export default node;
