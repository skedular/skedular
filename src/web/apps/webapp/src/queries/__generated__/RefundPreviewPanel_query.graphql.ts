/**
 * @generated SignedSource<<d807e59373f1809ebb37a11c05134439>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type RefundPreviewPanel_query$data = {
  readonly marketplaceBookingRefundPreview: {
    readonly baseAmount: any | null | undefined;
    readonly currencyToDisplay: string;
    readonly isRefundable: boolean;
    readonly refundAmount: any | null | undefined;
    readonly refundPercentage: number;
  };
  readonly " $fragmentType": "RefundPreviewPanel_query";
};
export type RefundPreviewPanel_query$key = {
  readonly " $data"?: RefundPreviewPanel_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"RefundPreviewPanel_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "bookingId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "RefundPreviewPanel_query",
  "selections": [
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
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "ad81603680c40a35c6d791aa2f495773";

export default node;
