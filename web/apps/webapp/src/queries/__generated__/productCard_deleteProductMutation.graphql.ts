/**
 * @generated SignedSource<<5cb8233db0405a492177c64bd7770a89>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type DeleteProductInput = {
  clientMutationId?: string | null | undefined;
  id: string;
};
export type productCard_deleteProductMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductInput;
};
export type productCard_deleteProductMutation$data = {
  readonly deleteProduct: {
    readonly product: {
      readonly id: string;
    };
  } | null | undefined;
};
export type productCard_deleteProductMutation = {
  response: productCard_deleteProductMutation$data;
  variables: productCard_deleteProductMutation$variables;
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
    "name": "productCard_deleteProductMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductPayload",
        "kind": "LinkedField",
        "name": "deleteProduct",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductDetails",
            "kind": "LinkedField",
            "name": "product",
            "plural": false,
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
    "name": "productCard_deleteProductMutation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductPayload",
        "kind": "LinkedField",
        "name": "deleteProduct",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "ProductDetails",
            "kind": "LinkedField",
            "name": "product",
            "plural": false,
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
    "cacheID": "3ed2573559263d6d2293ba01055882a6",
    "id": null,
    "metadata": {},
    "name": "productCard_deleteProductMutation",
    "operationKind": "mutation",
    "text": "mutation productCard_deleteProductMutation(\n  $input: DeleteProductInput!\n) {\n  deleteProduct(input: $input) {\n    product {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d1b1c9ebfeb4e1fba543cd0cb6ba9bd3";

export default node;
