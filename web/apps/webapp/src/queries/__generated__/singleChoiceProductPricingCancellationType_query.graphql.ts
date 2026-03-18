/**
 * @generated SignedSource<<f4d062242f0581103f9317a63b3cd13a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type ProductPricingCancellationPolicyType = "FULL_REFUND_BEFORE_CUTOFF" | "NOT_SET" | "NO_CANCELLATION" | "TIERED_REFUND" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceProductPricingCancellationType_query$data = {
  readonly productPricingCancellationTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: ProductPricingCancellationPolicyType;
  }>;
  readonly " $fragmentType": "singleChoiceProductPricingCancellationType_query";
};
export type singleChoiceProductPricingCancellationType_query$key = {
  readonly " $data"?: singleChoiceProductPricingCancellationType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceProductPricingCancellationType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceProductPricingCancellationType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "ProductPricingCancellationTypeDetails",
      "kind": "LinkedField",
      "name": "productPricingCancellationTypes",
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

(node as any).hash = "7f07739748d8af75cedb5ab0168bb878";

export default node;
