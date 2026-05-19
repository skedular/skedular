/**
 * @generated SignedSource<<a1ead01579a2ceb30695846925d59eea>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesProductPricingBillingModes_query$data = {
  readonly productPricingBillingModes: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingBillingMode;
  }>;
  readonly " $fragmentType": "multipleChoicesProductPricingBillingModes_query";
};
export type multipleChoicesProductPricingBillingModes_query$key = {
  readonly " $data"?: multipleChoicesProductPricingBillingModes_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesProductPricingBillingModes_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesProductPricingBillingModes_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingBillingModeDetails",
      "kind": "LinkedField",
      "name": "productPricingBillingModes",
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

(node as any).hash = "409bb5f96251de982f93fa5fe485df77";

export default node;
