/**
 * @generated SignedSource<<860a526b9662c9a28b76c6153ea935b5>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingBillingMode = "IN_ARREARS" | "NOT_SET" | "UPFRONT" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceProductPricingBillingMode_query$data = {
  readonly productPricingBillingModes: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingBillingMode;
  }>;
  readonly " $fragmentType": "singleChoiceProductPricingBillingMode_query";
};
export type singleChoiceProductPricingBillingMode_query$key = {
  readonly " $data"?: singleChoiceProductPricingBillingMode_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceProductPricingBillingMode_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceProductPricingBillingMode_query",
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

(node as any).hash = "af755e08866cd0dad40a5d63a02e547c";

export default node;
