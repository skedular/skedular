/**
 * @generated SignedSource<<75f1115230c125e90503c4ed7d6bf505>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myDetails_customerPaymentMethodsDetails_refetchableFragment$variables = Record<PropertyKey, never>;
export type myDetails_customerPaymentMethodsDetails_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myDetails_customerPaymentMethodsDetails_query">;
};
export type myDetails_customerPaymentMethodsDetails_refetchableFragment = {
  response: myDetails_customerPaymentMethodsDetails_refetchableFragment$data;
  variables: myDetails_customerPaymentMethodsDetails_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "myDetails_customerPaymentMethodsDetails_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myDetails_customerPaymentMethodsDetails_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "myDetails_customerPaymentMethodsDetails_refetchableFragment",
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
              (v0/*: any*/),
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
          },
          (v0/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "3ad9f31c5442bf1d22b12429f08fc1a4",
    "id": null,
    "metadata": {},
    "name": "myDetails_customerPaymentMethodsDetails_refetchableFragment",
    "operationKind": "query",
    "text": "query myDetails_customerPaymentMethodsDetails_refetchableFragment {\n  ...myDetails_customerPaymentMethodsDetails_query\n}\n\nfragment myDetails_customerPaymentMethodsDetails_query on Query {\n  me {\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "b2b2dbce2d35bcfdc3131a04e4a6feaa";

export default node;
