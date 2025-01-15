/**
 * @generated SignedSource<<ad20b95d3d35629551780d63f0515e73>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customTagCard_OrganizationTagDetails$data = {
  readonly color: string | null | undefined;
  readonly description: string | null | undefined;
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

(node as any).hash = "8edad052a79f7022875cdbf678dba2b2";

export default node;
