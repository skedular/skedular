/**
 * @generated SignedSource<<9f853b76b3baf74f9094257498dcb506>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type myBookingCard_query$data = {
  readonly paymentStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentStatus;
  }>;
  readonly " $fragmentType": "myBookingCard_query";
};
export type myBookingCard_query$key = {
  readonly " $data"?: myBookingCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookingCard_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "myBookingCard_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentStatusDetails",
      "kind": "LinkedField",
      "name": "paymentStatuses",
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

(node as any).hash = "6ae94a22342f0f32599020242d9d804e";

export default node;
