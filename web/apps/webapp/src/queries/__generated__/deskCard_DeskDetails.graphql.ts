/**
 * @generated SignedSource<<867b60d664021c13db0c995daf8d1dac>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type deskCard_DeskDetails$data = {
  readonly deactivated: boolean;
  readonly deskTypes: ReadonlyArray<{
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly id: string;
  readonly name: string;
  readonly requireBookingApproval: boolean;
  readonly zones: ReadonlyArray<{
    readonly name: string | null | undefined;
    readonly uniqueId: string;
  }>;
  readonly " $fragmentType": "deskCard_DeskDetails";
};
export type deskCard_DeskDetails$key = {
  readonly " $data"?: deskCard_DeskDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"deskCard_DeskDetails">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v0/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "deskCard_DeskDetails",
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
      "name": "deactivated",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "requireBookingApproval",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "deskTypes",
      "plural": true,
      "selections": (v1/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "Organization_OrganizationTagDetails",
      "kind": "LinkedField",
      "name": "zones",
      "plural": true,
      "selections": (v1/*: any*/),
      "storageKey": null
    }
  ],
  "type": "DeskDetails",
  "abstractKey": null
};
})();

(node as any).hash = "d55c7670ad137d40b02467ff94e5e998";

export default node;
