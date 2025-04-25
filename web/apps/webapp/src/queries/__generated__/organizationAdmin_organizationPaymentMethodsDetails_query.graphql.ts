/**
 * @generated SignedSource<<d5a1c6fc6f7d3195e4763b26dea28632>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_organizationPaymentMethodsDetails_query$data = {
  readonly organizationPaymentMethodsDetails: ReadonlyArray<{
    readonly cardBrand: string | null | undefined;
    readonly cardExpiryMonth: number | null | undefined;
    readonly cardExpiryYear: number | null | undefined;
    readonly cardLastFourDigit: string | null | undefined;
    readonly id: string;
  }>;
  readonly " $fragmentType": "organizationAdmin_organizationPaymentMethodsDetails_query";
};
export type organizationAdmin_organizationPaymentMethodsDetails_query$key = {
  readonly " $data"?: organizationAdmin_organizationPaymentMethodsDetails_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_organizationPaymentMethodsDetails_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./organizationAdmin_organizationPaymentMethodsDetails_refetchableFragment.graphql')
    }
  },
  "name": "organizationAdmin_organizationPaymentMethodsDetails_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "organizationId",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "PaymentMethod",
      "kind": "LinkedField",
      "name": "organizationPaymentMethodsDetails",
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

(node as any).hash = "18910cbd4b5458b8e98595d4d0a5bb33";

export default node;
