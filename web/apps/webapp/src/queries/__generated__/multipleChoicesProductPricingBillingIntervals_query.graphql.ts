/**
 * @generated SignedSource<<0a68eb2ad5834ce48dc3ab5afb7f002e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingBillingInterval = "FORTNIGHTLY" | "FULL_TERM" | "MONTHLY" | "NOT_SET" | "PER_BOOKING" | "WEEKLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesProductPricingBillingIntervals_query$data = {
  readonly productPricingBillingIntervals: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingBillingInterval;
  }>;
  readonly " $fragmentType": "multipleChoicesProductPricingBillingIntervals_query";
};
export type multipleChoicesProductPricingBillingIntervals_query$key = {
  readonly " $data"?: multipleChoicesProductPricingBillingIntervals_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesProductPricingBillingIntervals_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesProductPricingBillingIntervals_query",
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

(node as any).hash = "6b2d76e1672b807cc2bff08798d1f422";

export default node;
