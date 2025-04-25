/**
 * @generated SignedSource<<63608e7f5347c520acb835b39a2d825e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myDetails_myPaymentMethodsDetails_refetchableFragment$variables = Record<PropertyKey, never>;
export type myDetails_myPaymentMethodsDetails_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myDetails_myPaymentMethodsDetails_query">;
};
export type myDetails_myPaymentMethodsDetails_refetchableFragment = {
  response: myDetails_myPaymentMethodsDetails_refetchableFragment$data;
  variables: myDetails_myPaymentMethodsDetails_refetchableFragment$variables;
};

const node: ConcreteRequest = {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "myDetails_myPaymentMethodsDetails_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myDetails_myPaymentMethodsDetails_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "myDetails_myPaymentMethodsDetails_refetchableFragment",
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
    ]
  },
  "params": {
    "cacheID": "3a7b65cff0af0939df0c193ab0f443bf",
    "id": null,
    "metadata": {},
    "name": "myDetails_myPaymentMethodsDetails_refetchableFragment",
    "operationKind": "query",
    "text": "query myDetails_myPaymentMethodsDetails_refetchableFragment {\n  ...myDetails_myPaymentMethodsDetails_query\n}\n\nfragment myDetails_myPaymentMethodsDetails_query on Query {\n  myPaymentMethodsDetails {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n"
  }
};

(node as any).hash = "164789694e34222615e1abbc1c4b7e4d";

export default node;
