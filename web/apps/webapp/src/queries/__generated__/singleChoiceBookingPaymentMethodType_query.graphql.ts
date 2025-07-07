/**
 * @generated SignedSource<<7c37b7e4ae4e6438516ba9f34451803b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceBookingPaymentMethodType_query$data = {
  readonly paymentMethodTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentMethod;
  }>;
  readonly " $fragmentType": "singleChoiceBookingPaymentMethodType_query";
};
export type singleChoiceBookingPaymentMethodType_query$key = {
  readonly " $data"?: singleChoiceBookingPaymentMethodType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceBookingPaymentMethodType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceBookingPaymentMethodType_query",
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

(node as any).hash = "f552b2d533d1cde50502165c0677dc0a";

export default node;
