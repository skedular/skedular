/**
 * @generated SignedSource<<54b7c06aa65737841cb956f3702cb206>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type deskTypeCard_OrganizationTagDetails$data = {
  readonly id: string;
  readonly name: string;
  readonly " $fragmentType": "deskTypeCard_OrganizationTagDetails";
};
export type deskTypeCard_OrganizationTagDetails$key = {
  readonly " $data"?: deskTypeCard_OrganizationTagDetails$data;
  readonly " $fragmentSpreads": FragmentRefs<"deskTypeCard_OrganizationTagDetails">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "deskTypeCard_OrganizationTagDetails",
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

(node as any).hash = "1dc244464217405731548ded083e6a70";

export default node;
