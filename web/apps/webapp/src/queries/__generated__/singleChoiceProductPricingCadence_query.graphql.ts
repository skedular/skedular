/**
 * @generated SignedSource<<56e70818a065d7b0b92f07ddc285739b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingCadence = "DAILY_V1" | "FIVE_MONTHS_V1" | "FOUR_MONTHS_V1" | "HALF_DAY_V1" | "MONTHLY_V1" | "NOT_SET" | "ONE_TIME_V1" | "PER15_MINUTE_V1" | "PER30_MINUTE_V1" | "PER_HOUR_V1" | "PER_MINUTE_V1" | "QUARTERLY_V1" | "SIX_MONTHS_V1" | "TWO_MONTHS_V1" | "WEEKLY_V1" | "YEARLY_V1" | "%future added value";
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
