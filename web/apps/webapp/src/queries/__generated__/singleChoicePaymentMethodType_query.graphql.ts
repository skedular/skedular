/**
 * @generated SignedSource<<b47529d1ddaa3167bcbd17c3f6a4cc19>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoicePaymentMethodType_query$data = {
  readonly paymentMethodTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentMethod;
  }>;
  readonly " $fragmentType": "singleChoicePaymentMethodType_query";
};
export type singleChoicePaymentMethodType_query$key = {
  readonly " $data"?: singleChoicePaymentMethodType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoicePaymentMethodType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoicePaymentMethodType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentMethodTypeDetails",
      "kind": "LinkedField",
      "name": "paymentMethodTypes",
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

(node as any).hash = "d8ff446f14425efdfdece10dc895fa55";

export default node;
