/**
 * @generated SignedSource<<b79d86bf3f1b5b4a205576be13e4a6ec>>
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
export type organizationMarketplaceSetup_deleteProductsMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductsInput;
};
export type organizationMarketplaceSetup_deleteProductsMutation$data = {
  readonly deleteProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMarketplaceSetup_deleteProductsMutation = {
  response: organizationMarketplaceSetup_deleteProductsMutation$data;
  variables: organizationMarketplaceSetup_deleteProductsMutation$variables;
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
    "name": "organizationMarketplaceSetup_deleteProductsMutation",
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
    "name": "organizationMarketplaceSetup_deleteProductsMutation",
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
    "cacheID": "898506d327f9a3e90d53ba1c6030f56e",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteProductsMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteProductsMutation(\n  $input: DeleteProductsInput!\n) {\n  deleteProducts(input: $input) {\n    products {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6ae84f96e5baed04552236ad6ad24734";

export default node;
