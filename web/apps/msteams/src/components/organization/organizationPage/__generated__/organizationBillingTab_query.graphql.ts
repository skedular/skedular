/**
 * @generated SignedSource<<65a357ee0a5019d259d0eb75c0cb2e30>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationBillingTab_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"organizationBillingInfo_query" | "organizationPaymentMethods_query">;
  readonly " $fragmentType": "organizationBillingTab_query";
};
export type organizationBillingTab_query$key = {
  readonly " $data"?: organizationBillingTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBillingTab_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationBillingTab_query",
  "selections": [
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationPaymentMethods_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationBillingInfo_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "51efc8acb60e4aad1db11ca2613698ca";

export default node;
