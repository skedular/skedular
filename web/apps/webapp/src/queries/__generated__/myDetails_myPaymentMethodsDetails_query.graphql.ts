/**
 * @generated SignedSource<<a64695a560ffa1d165a2860ef671231b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myDetails_myPaymentMethodsDetails_query$data = {
  readonly myPaymentMethodsDetails: ReadonlyArray<{
    readonly cardBrand: string | null | undefined;
    readonly cardExpiryMonth: number | null | undefined;
    readonly cardExpiryYear: number | null | undefined;
    readonly cardLastFourDigit: string | null | undefined;
    readonly id: string;
  }>;
  readonly " $fragmentType": "myDetails_myPaymentMethodsDetails_query";
};
export type myDetails_myPaymentMethodsDetails_query$key = {
  readonly " $data"?: myDetails_myPaymentMethodsDetails_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myDetails_myPaymentMethodsDetails_query">;
};

import myDetails_myPaymentMethodsDetails_refetchableFragment_graphql from './myDetails_myPaymentMethodsDetails_refetchableFragment.graphql';

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": myDetails_myPaymentMethodsDetails_refetchableFragment_graphql
    }
  },
  "name": "myDetails_myPaymentMethodsDetails_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "PaymentMethod",
      "kind": "LinkedField",
      "name": "myPaymentMethodsDetails",
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
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "164789694e34222615e1abbc1c4b7e4d";

export default node;
