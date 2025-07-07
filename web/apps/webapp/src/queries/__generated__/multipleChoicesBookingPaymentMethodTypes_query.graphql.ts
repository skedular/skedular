/**
 * @generated SignedSource<<deda7394527993d960e9280da36db51e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingPaymentMethod = "BANK_TRANSFER" | "CARD" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesBookingPaymentMethodTypes_query$data = {
  readonly bookingPaymentMethodTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: BookingPaymentMethod;
  }>;
  readonly " $fragmentType": "multipleChoicesBookingPaymentMethodTypes_query";
};
export type multipleChoicesBookingPaymentMethodTypes_query$key = {
  readonly " $data"?: multipleChoicesBookingPaymentMethodTypes_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesBookingPaymentMethodTypes_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesBookingPaymentMethodTypes_query",
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

(node as any).hash = "860b0d1b1823eeaec0a32f6ed07fb021";

export default node;
