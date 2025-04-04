/**
 * @generated SignedSource<<e00b4b7e3a44177acefa4dfb90ae3266>>
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
export type organizationProducts_deleteProductMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductInput;
};
export type organizationProducts_deleteProductMutation$data = {
  readonly deleteProduct: {
    readonly product: {
      readonly id: string;
    };
  } | null | undefined;
};
export type organizationProducts_deleteProductMutation = {
  response: organizationProducts_deleteProductMutation$data;
  variables: organizationProducts_deleteProductMutation$variables;
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
    "name": "organizationProducts_deleteProductMutation",
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
    "name": "organizationProducts_deleteProductMutation",
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
    "cacheID": "2c76002514620dbb854778bb0d294c72",
    "id": null,
    "metadata": {},
    "name": "organizationProducts_deleteProductMutation",
    "operationKind": "mutation",
    "text": "mutation organizationProducts_deleteProductMutation(\n  $input: DeleteProductInput!\n) {\n  deleteProduct(input: $input) {\n    product {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "89479d9275574dc201b21c7c53b41ad8";

export default node;
