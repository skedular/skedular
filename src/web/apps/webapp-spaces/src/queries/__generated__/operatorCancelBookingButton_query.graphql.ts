/**
 * @generated SignedSource<<a6daf4b069126653ff942ad3c1c87624>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type operatorCancelBookingButton_query$variables = {
  bookingId: string;
};
export type operatorCancelBookingButton_query$data = {
  readonly marketplaceBookingRefundPreview: {
    readonly baseAmount: any | null | undefined;
    readonly currencyToDisplay: string;
    readonly refundAmount: any | null | undefined;
  };
};
export type operatorCancelBookingButton_query = {
  response: operatorCancelBookingButton_query$data;
  variables: operatorCancelBookingButton_query$variables;
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "bookingId",
        "variableName": "bookingId"
      }
    ],
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
        "name": "currencyToDisplay",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "operatorCancelBookingButton_query",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "operatorCancelBookingButton_query",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "9d1ed96235a4226a6e41b841bbaebc5a",
    "id": null,
    "metadata": {},
    "name": "operatorCancelBookingButton_query",
    "operationKind": "query",
    "text": "query operatorCancelBookingButton_query(\n  $bookingId: String!\n) {\n  marketplaceBookingRefundPreview(bookingId: $bookingId) {\n    refundAmount\n    baseAmount\n    currencyToDisplay\n  }\n}\n"
  }
};
})();

(node as any).hash = "66159155ac935da5aa61ba7f2f645c0e";

export default node;
