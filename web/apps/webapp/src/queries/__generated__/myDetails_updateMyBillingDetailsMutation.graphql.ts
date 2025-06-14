/**
 * @generated SignedSource<<d115f28406af5a9686a579d52662762c>>
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
export type myDetails_updateMyBillingDetailsMutation$variables = {
  input: UpdateMyBillingDetailsInput;
};
export type myDetails_updateMyBillingDetailsMutation$data = {
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
  } | null | undefined;
};
export type myDetails_updateMyBillingDetailsMutation$rawResponse = {
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
  } | null | undefined;
};
export type myDetails_updateMyBillingDetailsMutation = {
  rawResponse: myDetails_updateMyBillingDetailsMutation$rawResponse;
  response: myDetails_updateMyBillingDetailsMutation$data;
  variables: myDetails_updateMyBillingDetailsMutation$variables;
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
    "name": "myDetails_updateMyBillingDetailsMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myDetails_updateMyBillingDetailsMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "0f0fb1ed4159f0db8f45aae99e5b45b8",
    "id": null,
    "metadata": {},
    "name": "myDetails_updateMyBillingDetailsMutation",
    "operationKind": "mutation",
    "text": "mutation myDetails_updateMyBillingDetailsMutation(\n  $input: UpdateMyBillingDetailsInput!\n) {\n  updateMyBillingDetails(input: $input) {\n    customer {\n      id\n      billingDetails {\n        id\n        companyName\n        email\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "00d87f973fb545d6f75b18d88986e68c";

export default node;
