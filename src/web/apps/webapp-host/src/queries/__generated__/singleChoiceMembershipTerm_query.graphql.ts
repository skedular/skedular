/**
 * @generated SignedSource<<88e3174d7717702cd830964da68af6d4>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type MembershipTerm = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "MONTHLY" | "NOT_SET" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceMembershipTerm_query$data = {
  readonly membershipTerms: ReadonlyArray<{
    readonly name: string;
    readonly type: MembershipTerm;
  }>;
  readonly " $fragmentType": "singleChoiceMembershipTerm_query";
};
export type singleChoiceMembershipTerm_query$key = {
  readonly " $data"?: singleChoiceMembershipTerm_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceMembershipTerm_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceMembershipTerm_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "MembershipTermDetails",
      "kind": "LinkedField",
      "name": "membershipTerms",
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

(node as any).hash = "60b1adfbcfee53abbc4742764e18e7ff";

export default node;
