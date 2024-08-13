/**
 * @generated SignedSource<<9010b18ceb6de1eec397f11a98d2f11b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamCard_TeamDetails$data = {
  readonly about: string | null | undefined;
  readonly canDelete: boolean;
  readonly canModify: boolean;
  readonly hasFutureBooking: boolean;
  readonly id: string;
  readonly name: string;
  readonly organization: {
    readonly name: string;
    readonly uniqueId: string;
  } | null | undefined;
  readonly " $fragmentType": "teamCard_TeamDetails";
};
export type teamCard_TeamDetails$key = {
  readonly " $data"?: teamCard_TeamDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamCard_TeamDetails">;
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
  "name": "teamCard_TeamDetails",
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
      "name": "about",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamOrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "uniqueId",
          "storageKey": null
        },
        (v0/*: any*/)
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
    }
  ],
  "type": "TeamDetails",
  "abstractKey": null
};
})();

(node as any).hash = "41d19638dba44299d0a683be6b9a2524";

export default node;
