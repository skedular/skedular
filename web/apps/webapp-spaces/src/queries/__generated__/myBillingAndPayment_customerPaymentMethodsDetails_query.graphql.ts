/**
 * @generated SignedSource<<662cbbc0cc302dbf61d15c920d57b08d>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBillingAndPayment_customerPaymentMethodsDetails_query$data = {
  readonly me: {
    readonly paymentMethods: ReadonlyArray<{
      readonly cardBrand: string | null | undefined;
      readonly cardExpiryMonth: number | null | undefined;
      readonly cardExpiryYear: number | null | undefined;
      readonly cardLastFourDigit: string | null | undefined;
      readonly id: string;
    }>;
  };
  readonly " $fragmentType": "myBillingAndPayment_customerPaymentMethodsDetails_query";
};
export type myBillingAndPayment_customerPaymentMethodsDetails_query$key = {
  readonly " $data"?: myBillingAndPayment_customerPaymentMethodsDetails_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBillingAndPayment_customerPaymentMethodsDetails_query">;
};

import myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment_graphql from './myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment_graphql
    }
  },
  "name": "myBillingAndPayment_customerPaymentMethodsDetails_query",
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

(node as any).hash = "111ea6d9454b93b23f59a94cdb4e64c1";

export default node;
