/**
 * @generated SignedSource<<f2244c5fa2bee53198480404aeae535f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationMemberVisibilityPolicy = "FULL_ACCESS" | "LIMITED_ACCESS" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceOrganizationMemberVisibilityPolicyquery$data = {
  readonly organizationMemberVisibilityPolicies: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationMemberVisibilityPolicy;
  }>;
  readonly " $fragmentType": "singleChoiceOrganizationMemberVisibilityPolicyquery";
};
export type singleChoiceOrganizationMemberVisibilityPolicyquery$key = {
  readonly " $data"?: singleChoiceOrganizationMemberVisibilityPolicyquery$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceOrganizationMemberVisibilityPolicyquery">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceOrganizationMemberVisibilityPolicyquery",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationMemberVisibilityPolicyDetails",
      "kind": "LinkedField",
      "name": "organizationMemberVisibilityPolicies",
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

(node as any).hash = "dfa8e997fe09e57a300ef90ee0b5cc49";

export default node;
