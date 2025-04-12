/**
 * @generated SignedSource<<4b26aa6ddd8318491740f547bbe199f0>>
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
export type organizationProducts_deleteProductsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductsInput;
};
export type organizationProducts_deleteProductsMutation$data = {
  readonly deleteProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationProducts_deleteProductsMutation = {
  response: organizationProducts_deleteProductsMutation$data;
  variables: organizationProducts_deleteProductsMutation$variables;
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
    "name": "organizationProducts_deleteProductsMutation",
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
    "name": "organizationProducts_deleteProductsMutation",
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
    "cacheID": "0c0e1499f62b5bdc42490f42afb72233",
    "id": null,
    "metadata": {},
    "name": "organizationProducts_deleteProductsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationProducts_deleteProductsMutation(\n  $input: DeleteProductsInput!\n) {\n  deleteProducts(input: $input) {\n    products {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "0a23a044262cdb752169587c7e345380";

export default node;
