/**
 * @generated SignedSource<<f3752648c16fc133910c59540404fa1d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeactivateProductsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type productCard_deactivateProductsMutation$variables = {
  input: DeactivateProductsInput;
};
export type productCard_deactivateProductsMutation$data = {
  readonly deactivateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type productCard_deactivateProductsMutation$rawResponse = {
  readonly deactivateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type productCard_deactivateProductsMutation = {
  rawResponse: productCard_deactivateProductsMutation$rawResponse;
  response: productCard_deactivateProductsMutation$data;
  variables: productCard_deactivateProductsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "ProductsPayload",
    "kind": "LinkedField",
    "name": "deactivateProducts",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "products",
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
            "name": "inactive",
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
    "name": "productCard_deactivateProductsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "productCard_deactivateProductsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "467423d4a4bc2c38c06708a906431957",
    "id": null,
    "metadata": {},
    "name": "productCard_deactivateProductsMutation",
    "operationKind": "mutation",
    "text": "mutation productCard_deactivateProductsMutation(\n  $input: DeactivateProductsInput!\n) {\n  deactivateProducts(input: $input) {\n    products {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "17c2bcc78e8736343c5a16e1af1a48f8";

export default node;
