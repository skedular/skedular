/**
 * @generated SignedSource<<5d42bc7221fd6d9c2fbaaf2496e5e7c0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customTagCard_OrganizationTagDetails$data = {
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "customTagCard_OrganizationTagDetails";
};
export type customTagCard_OrganizationTagDetails$key = {
  readonly " $data"?: customTagCard_OrganizationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"customTagCard_OrganizationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "customTagCard_OrganizationTagDetails",
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
    }
  ],
  "type": "OrganizationTagDetails",
  "abstractKey": null
};

(node as any).hash = "0380a850f05888f81994032c906b12d7";

export default node;
