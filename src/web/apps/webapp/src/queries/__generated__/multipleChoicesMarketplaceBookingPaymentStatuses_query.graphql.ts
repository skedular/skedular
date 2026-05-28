/**
 * @generated SignedSource<<07a239ac5f9ad437cbe708194bf491fc>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type multipleChoicesMarketplaceBookingPaymentStatuses_query$data = {
  readonly marketplaceBookingPaymentStatuses: ReadonlyArray<{
    readonly name: string;
    readonly type: PaymentStatus;
  }>;
  readonly " $fragmentType": "multipleChoicesMarketplaceBookingPaymentStatuses_query";
};
export type multipleChoicesMarketplaceBookingPaymentStatuses_query$key = {
  readonly " $data"?: multipleChoicesMarketplaceBookingPaymentStatuses_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesMarketplaceBookingPaymentStatuses_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "multipleChoicesMarketplaceBookingPaymentStatuses_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "MarketplaceBookingPaymentStatusDetails",
      "kind": "LinkedField",
      "name": "marketplaceBookingPaymentStatuses",
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

(node as any).hash = "a03947d4189490431c467ec92839dc36";

export default node;
