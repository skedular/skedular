/**
 * @generated SignedSource<<83b8ebf49ca470acd7374bfcffd8c599>>
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
  readonly id: string;
  readonly locationTags: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly name: string;
  readonly requireBookingApproval: boolean;
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
  "name": "id",
  "storageKey": null
},
v1 = {
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
  "name": "deskCard_DeskDetails",
  "selections": [
    (v0/*: any*/),
    (v1/*: any*/),
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
      "concreteType": "LocationTagDetails",
      "kind": "LinkedField",
      "name": "locationTags",
      "plural": true,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/)
      ],
      "storageKey": null
    }
  ],
  "type": "DeskDetails",
  "abstractKey": null
};
})();

(node as any).hash = "62f05a4e95ea7c13f995bc72d6dd7e54";

export default node;
