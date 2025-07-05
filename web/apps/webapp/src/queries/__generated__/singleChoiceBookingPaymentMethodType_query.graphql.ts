/**
 * @generated SignedSource<<35aea01ec10aabdf732164415bf0b6af>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingPaymentMethod = "BANK_ACCOUNT" | "CARD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceBookingPaymentMethodType_query$data = {
  readonly bookingPaymentMethodTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: BookingPaymentMethod;
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
      "concreteType": "BookingPaymentMethodTypeDetails",
      "kind": "LinkedField",
      "name": "bookingPaymentMethodTypes",
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

(node as any).hash = "3d6d7bdea9935f31cd01773e0eb7a003";

export default node;
