/**
 * @generated SignedSource<<774067cc16ad860e048e3c65d1fe89ec>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type UpdateMyBillingDetailsInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  clientMutationId?: string | null | undefined;
  companyName?: string | null | undefined;
  country: string;
  email: string;
  id: string;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type myBillingAndPayment_updateMyBillingDetailsMutation$variables = {
  input: UpdateMyBillingDetailsInput;
};
export type myBillingAndPayment_updateMyBillingDetailsMutation$data = {
  readonly updateMyBillingDetails: {
    readonly customer: {
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
  };
};
export type myBillingAndPayment_updateMyBillingDetailsMutation$rawResponse = {
  readonly updateMyBillingDetails: {
    readonly customer: {
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
  };
};
export type myBillingAndPayment_updateMyBillingDetailsMutation = {
  rawResponse: myBillingAndPayment_updateMyBillingDetailsMutation$rawResponse;
  response: myBillingAndPayment_updateMyBillingDetailsMutation$data;
  variables: myBillingAndPayment_updateMyBillingDetailsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "CustomerPayload",
    "kind": "LinkedField",
    "name": "updateMyBillingDetails",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "customer",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerBillingDetails",
            "kind": "LinkedField",
            "name": "billingDetails",
            "plural": false,
            "selections": [
              (v1/*: any*/),
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
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myBillingAndPayment_updateMyBillingDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myBillingAndPayment_updateMyBillingDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "0006865f9ad5b8f83043c674c307ef15",
    "id": null,
    "metadata": {},
    "name": "myBillingAndPayment_updateMyBillingDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation myBillingAndPayment_updateMyBillingDetailsMutation(\n  $input: UpdateMyBillingDetailsInput!\n) {\n  updateMyBillingDetails(input: $input) {\n    customer {\n      id\n      billingDetails {\n        id\n        companyName\n        email\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "a17f129217eac070725093f35e6b4cb8";

export default node;
