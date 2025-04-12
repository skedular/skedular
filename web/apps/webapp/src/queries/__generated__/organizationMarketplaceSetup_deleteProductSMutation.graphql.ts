/**
 * @generated SignedSource<<d9dc8b9d6782661ebf7447909bed99cf>>
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
export type organizationMarketplaceSetup_deleteProductSMutation$variables = {
  connectionIds: ReadonlyArray<string>;
  input: DeleteProductsInput;
};
export type organizationMarketplaceSetup_deleteProductSMutation$data = {
  readonly deleteProducts: {
    readonly products: ReadonlyArray<{
      readonly id: string;
    }>;
  } | null | undefined;
};
export type organizationMarketplaceSetup_deleteProductSMutation = {
  response: organizationMarketplaceSetup_deleteProductSMutation$data;
  variables: organizationMarketplaceSetup_deleteProductSMutation$variables;
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
    "name": "organizationMarketplaceSetup_deleteProductSMutation",
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
    "name": "organizationMarketplaceSetup_deleteProductSMutation",
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
    "cacheID": "d80468ec902c6942502ff1cc66b7c3dd",
    "id": null,
    "metadata": {},
    "name": "organizationMarketplaceSetup_deleteProductSMutation",
    "operationKind": "mutation",
    "text": "mutation organizationMarketplaceSetup_deleteProductSMutation(\n  $input: DeleteProductsInput!\n) {\n  deleteProducts(input: $input) {\n    products {\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b73dda7170a376f100eade8827b98a9b";

export default node;
