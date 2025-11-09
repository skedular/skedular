/**
 * @generated SignedSource<<6dbf56dcb9dcd20c387113b493f06ebf>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type ActivateProductsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type productCard_activateProductsMutation$variables = {
  input: ActivateProductsInput;
};
export type productCard_activateProductsMutation$data = {
  readonly activateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type productCard_activateProductsMutation$rawResponse = {
  readonly activateProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
      readonly inactive: boolean;
    }>;
  };
};
export type productCard_activateProductsMutation = {
  rawResponse: productCard_activateProductsMutation$rawResponse;
  response: productCard_activateProductsMutation$data;
  variables: productCard_activateProductsMutation$variables;
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
    "name": "activateProducts",
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
    "name": "productCard_activateProductsMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "productCard_activateProductsMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "df0e4ae5a871b2d21925d01184e73c25",
    "id": null,
    "metadata": {},
    "name": "productCard_activateProductsMutation",
    "operationKind": "mutation",
    "text": "mutation productCard_activateProductsMutation(\n  $input: ActivateProductsInput!\n) {\n  activateProducts(input: $input) {\n    products {\n      id\n      inactive\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "cda43e50f7227bcbf7940f4f31300104";

export default node;
