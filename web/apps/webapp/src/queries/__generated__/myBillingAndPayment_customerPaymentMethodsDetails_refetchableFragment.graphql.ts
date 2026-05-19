/**
 * @generated SignedSource<<2a135aa5551c4023ad2a46fa2c18febc>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment$variables = Record<PropertyKey, never>;
export type myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myBillingAndPayment_customerPaymentMethodsDetails_query">;
};
export type myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment = {
  response: myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment$data;
  variables: myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment$variables;
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
    "name": "myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myBillingAndPayment_customerPaymentMethodsDetails_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment",
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
              (v0/*:: as any*/),
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
          (v0/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "9b1aaddd9d5b17065e33a3ccd482824a",
    "id": null,
    "metadata": {},
    "name": "myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment",
    "operationKind": "query",
    "text": "query myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment {\n  ...myBillingAndPayment_customerPaymentMethodsDetails_query\n}\n\nfragment myBillingAndPayment_customerPaymentMethodsDetails_query on Query {\n  me {\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "111ea6d9454b93b23f59a94cdb4e64c1";

export default node;
