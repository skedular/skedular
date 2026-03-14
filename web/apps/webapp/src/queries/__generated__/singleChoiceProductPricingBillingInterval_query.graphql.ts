/**
 * @generated SignedSource<<036b02f430598a866fa67e0457874ac5>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingBillingInterval = "FORTNIGHTLY" | "FULL_TERM" | "MONTHLY" | "NOT_SET" | "PER_BOOKING" | "WEEKLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceProductPricingBillingInterval_query$data = {
  readonly productPricingBillingIntervals: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingBillingInterval;
  }>;
  readonly " $fragmentType": "singleChoiceProductPricingBillingInterval_query";
};
export type singleChoiceProductPricingBillingInterval_query$key = {
  readonly " $data"?: singleChoiceProductPricingBillingInterval_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceProductPricingBillingInterval_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceProductPricingBillingInterval_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingBillingIntervalDetails",
      "kind": "LinkedField",
      "name": "productPricingBillingIntervals",
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

(node as any).hash = "4ab0723fc7c6fe74a3ec172f1cecfa3f";

export default node;
