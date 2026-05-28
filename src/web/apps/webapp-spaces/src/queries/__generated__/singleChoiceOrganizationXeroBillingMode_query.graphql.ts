/**
 * @generated SignedSource<<ae34c904c7ef76fd56236d70c2f54cb5>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationXeroBillingMode = "DISABLED" | "ENABLED" | "REPEATING_INVOICES" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceOrganizationXeroBillingMode_query$data = {
  readonly organizationXeroBillingModes: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationXeroBillingMode;
  }>;
  readonly " $fragmentType": "singleChoiceOrganizationXeroBillingMode_query";
};
export type singleChoiceOrganizationXeroBillingMode_query$key = {
  readonly " $data"?: singleChoiceOrganizationXeroBillingMode_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceOrganizationXeroBillingMode_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceOrganizationXeroBillingMode_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationXeroBillingModeDetails",
      "kind": "LinkedField",
      "name": "organizationXeroBillingModes",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
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
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "667b190a63c27ac7ad7b764982ffc6c0";

export default node;
