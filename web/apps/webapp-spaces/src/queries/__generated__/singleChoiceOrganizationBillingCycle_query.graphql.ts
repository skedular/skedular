/**
 * @generated SignedSource<<a0fef239da985691f3d9ffb75d5e2983>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceOrganizationBillingCycle_query$data = {
  readonly organizationBillingCycles: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationBillingCycle;
  }>;
  readonly " $fragmentType": "singleChoiceOrganizationBillingCycle_query";
};
export type singleChoiceOrganizationBillingCycle_query$key = {
  readonly " $data"?: singleChoiceOrganizationBillingCycle_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceOrganizationBillingCycle_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceOrganizationBillingCycle_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationBillingCycleDetails",
      "kind": "LinkedField",
      "name": "organizationBillingCycles",
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

(node as any).hash = "585cbbde226747bf6f62a0219ac47839";

export default node;
