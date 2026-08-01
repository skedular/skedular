/**
 * @generated SignedSource<<b437bf2f2c69f5e10ec82c9183b44570>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type MarketplaceBookingCancellationConfirmationDialog_query$variables = {
  bookingId: string;
};
export type MarketplaceBookingCancellationConfirmationDialog_query$data = {
  readonly " $fragmentSpreads": FragmentRefs<"RefundPreviewPanel_query">;
};
export type MarketplaceBookingCancellationConfirmationDialog_query = {
  response: MarketplaceBookingCancellationConfirmationDialog_query$data;
  variables: MarketplaceBookingCancellationConfirmationDialog_query$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "bookingId",
    "variableName": "bookingId"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "MarketplaceBookingCancellationConfirmationDialog_query",
    "selections": [
      {
        "args": (v1/*:: as any*/),
        "kind": "FragmentSpread",
        "name": "RefundPreviewPanel_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "MarketplaceBookingCancellationConfirmationDialog_query",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "MarketplaceRefundPreviewDetails",
        "kind": "LinkedField",
        "name": "marketplaceBookingRefundPreview",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "refundAmount",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "baseAmount",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "refundPercentage",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "currencyToDisplay",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "isRefundable",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "9e1706e85d2913a96dbbdf4dd42e75fd",
    "id": null,
    "metadata": {},
    "name": "MarketplaceBookingCancellationConfirmationDialog_query",
    "operationKind": "query",
    "text": "query MarketplaceBookingCancellationConfirmationDialog_query(\n  $bookingId: String!\n) {\n  ...RefundPreviewPanel_query_378Z3H\n}\n\nfragment RefundPreviewPanel_query_378Z3H on Query {\n  marketplaceBookingRefundPreview(bookingId: $bookingId) {\n    refundAmount\n    baseAmount\n    refundPercentage\n    currencyToDisplay\n    isRefundable\n  }\n}\n"
  }
};
})();

(node as any).hash = "7d9e1a65133ca9c3a926c5599de8c81f";

export default node;
