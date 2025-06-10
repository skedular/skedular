/**
 * @generated SignedSource<<bd2c981b81e208bd8b3c72ec7b26ed72>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myDetails_customerPaymentMethodsDetails_query$data = {
  readonly me: {
    readonly paymentMethods: ReadonlyArray<{
      readonly cardBrand: string | null | undefined;
      readonly cardExpiryMonth: number | null | undefined;
      readonly cardExpiryYear: number | null | undefined;
      readonly cardLastFourDigit: string | null | undefined;
      readonly id: string;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "myDetails_customerPaymentMethodsDetails_query";
};
export type myDetails_customerPaymentMethodsDetails_query$key = {
  readonly " $data"?: myDetails_customerPaymentMethodsDetails_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myDetails_customerPaymentMethodsDetails_query">;
};

import myDetails_customerPaymentMethodsDetails_refetchableFragment_graphql from './myDetails_customerPaymentMethodsDetails_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": myDetails_customerPaymentMethodsDetails_refetchableFragment_graphql
    }
  },
  "name": "myDetails_customerPaymentMethodsDetails_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerPaymentMethod",
          "kind": "LinkedField",
          "name": "paymentMethods",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "id",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cardBrand",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cardExpiryMonth",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cardExpiryYear",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "cardLastFourDigit",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b2b2dbce2d35bcfdc3131a04e4a6feaa";

export default node;
