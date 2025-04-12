/**
 * @generated SignedSource<<49615f81017683a21a6f33704785a84c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteProductsInput = {
  clientMutationId?: string | null | undefined;
  ids: ReadonlyArray<string>;
};
export type productCard_deleteProductsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductsInput;
};
export type productCard_deleteProductsMutation$data = {
  readonly deleteProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type productCard_deleteProductsMutation = {
  response: productCard_deleteProductsMutation$data;
  variables: productCard_deleteProductsMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "connectionIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "input",
    "variableName": "input"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "productCard_deleteProductsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductsPayload",
        "kind": "LinkedField",
        "name": "deleteProducts",
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
              (v2/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ],
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "productCard_deleteProductsMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductsPayload",
        "kind": "LinkedField",
        "name": "deleteProducts",
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
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "filters": null,
                "handle": "deleteEdge",
                "key": "",
                "kind": "ScalarHandle",
                "name": "id",
                "handleArgs": [
                  {
                    "kind": "Variable",
                    "name": "connections",
                    "variableName": "connectionIds"
                  }
                ]
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
    "cacheID": "d90fb5f293c881b8a93b598a38827aa7",
    "id": null,
    "metadata": {},
    "name": "productCard_deleteProductsMutation",
    "operationKind": "mutation",
    "text": "mutation productCard_deleteProductsMutation(\n  $input: DeleteProductsInput!\n) {\n  deleteProducts(input: $input) {\n    products {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "ea625b12b78d9cb6a3f1990402c21283";

export default node;
