/**
 * @generated SignedSource<<ddbbdd4cf796523ac90c131079eeb78a>>
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
      readonly city: string | null | undefined;
      readonly companyName: string | null | undefined;
      readonly country: string;
      readonly countryCode: string | null | undefined;
      readonly email: string;
      readonly formattedAddress: string | null | undefined;
      readonly id: string;
      readonly latitude: number | null | undefined;
      readonly longitude: number | null | undefined;
      readonly osmId: string | null | undefined;
      readonly osmType: string | null | undefined;
      readonly placeId: string | null | undefined;
      readonly province: string | null | undefined;
      readonly suburb: string | null | undefined;
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
      "name": "osmType",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "osmId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "placeId",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "longitude",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "latitude",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "formattedAddress",
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "countryCode",
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
    "cacheID": "9cd7adccd8f2d9616cc420682eeb1ea8",
    "id": null,
    "metadata": {},
    "name": "myBillingAndPayment_rootQuery",
    "operationKind": "query",
    "text": "query myBillingAndPayment_rootQuery {\n  me {\n    id\n    billingDetails {\n      id\n      companyName\n      email\n      osmType\n      osmId\n      placeId\n      longitude\n      latitude\n      formattedAddress\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      countryCode\n    }\n  }\n  ...myBillingAndPayment_customerPaymentMethodsDetails_query\n}\n\nfragment myBillingAndPayment_customerPaymentMethodsDetails_query on Query {\n  me {\n    paymentMethods {\n      id\n      cardBrand\n      cardExpiryMonth\n      cardExpiryYear\n      cardLastFourDigit\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "72041798d6388728c4dd06986a6a4da6";

export default node;
