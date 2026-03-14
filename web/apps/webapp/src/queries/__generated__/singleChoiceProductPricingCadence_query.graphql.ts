/**
 * @generated SignedSource<<75b6ac00ff326f7262cf92cde2703190>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingCadence = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "HALF_DAY" | "MONTHLY" | "NOT_SET" | "ONE_TIME" | "PER15_MINUTES" | "PER30_MINUTES" | "PER_HOUR" | "PER_MINUTE" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceProductPricingCadence_query$data = {
  readonly productPricingCadences: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCadence;
  }>;
  readonly " $fragmentType": "singleChoiceProductPricingCadence_query";
};
export type singleChoiceProductPricingCadence_query$key = {
  readonly " $data"?: singleChoiceProductPricingCadence_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceProductPricingCadence_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceProductPricingCadence_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingCadenceDetails",
      "kind": "LinkedField",
      "name": "productPricingCadences",
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

(node as any).hash = "d5702259e66210e6fdbe35acb31d1f98";

export default node;
