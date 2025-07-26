/**
 * @generated SignedSource<<088f5cf7352bb6123b1392a4eafe121d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBillingAndPayment_rootQuery$variables = Record<PropertyKey, never>;
export type myBillingAndPayment_rootQuery$data = {
  readonly me: {
    readonly billingDetails: {
      readonly addressLine1: string;
      readonly addressLine2: string | null | undefined;
      readonly city: string;
      readonly companyName: string | null | undefined;
      readonly country: string;
      readonly email: string;
      readonly id: string;
      readonly province: string | null | undefined;
      readonly suburb: string;
      readonly zipcode: string;
    } | null | undefined;
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"myBillingAndPayment_customerPaymentMethodsDetails_query">;
};
export type myBillingAndPayment_rootQuery = {
  response: myBillingAndPayment_rootQuery$data;
  variables: myBillingAndPayment_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerBillingDetails",
  "kind": "LinkedField",
  "name": "billingDetails",
  "plural": false,
  "selections": [
    (v0/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "companyName",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "email",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "addressLine1",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "addressLine2",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "suburb",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "city",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "province",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "zipcode",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "country",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "myBillingAndPayment_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          (v1/*: any*/)
        ],
        "storageKey": null
      },
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
    "name": "myBillingAndPayment_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          (v1/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "12f022a541d03cddf5687978e45379d8",
    "id": null,
    "metadata": {},
    "name": "myBillingAndPayment_rootQuery",
    "operationKind": "query",
    "text": "query myBillingAndPayment_rootQuery {\n  me {\n    id\n    billingDetails {\n      id\n      companyName\n      email\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n    }\n  }\n  ...myBillingAndPayment_customerPaymentMethodsDetails_query\n}\n\nfragment myBillingAndPayment_customerPaymentMethodsDetails_query on Query {\n  me {\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a469c34e75dadce98ca9fcaa90f16878";

export default node;
