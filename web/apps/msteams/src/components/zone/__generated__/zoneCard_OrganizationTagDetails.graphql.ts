/**
 * @generated SignedSource<<7fecc07626f535526e1d76611d1ee9f7>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneCard_OrganizationTagDetails$data = {
  readonly color: string | null | undefined;
  readonly description: string | null | undefined;
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "zoneCard_OrganizationTagDetails";
};
export type zoneCard_OrganizationTagDetails$key = {
  readonly " $data"?: zoneCard_OrganizationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_OrganizationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "zoneCard_OrganizationTagDetails",
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
      "kind": "ScalarField",
      "name": "description",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "color",
      "storageKey": null
    }
  ],
  "type": "OrganizationTagDetails",
  "abstractKey": null
};

(node as any).hash = "d8e6eaf9e013810d1f2587ad54166f38";

export default node;
