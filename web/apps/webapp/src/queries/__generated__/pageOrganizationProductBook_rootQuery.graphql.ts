/**
 * @generated SignedSource<<87c5702d29e3c22bf16986bd3da5c8d8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationProductBook_rootQuery$variables = {
  productId: string;
};
export type pageOrganizationProductBook_rootQuery$data = {
  readonly product: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_query">;
};
export type pageOrganizationProductBook_rootQuery = {
  response: pageOrganizationProductBook_rootQuery$data;
  variables: pageOrganizationProductBook_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productId"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "productId"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationProductBook_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookProduct_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "pageOrganizationProductBook_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "ProductDetails",
        "kind": "LinkedField",
        "name": "product",
        "plural": false,
        "selections": [
          (v2/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "a6819e31ec195086d605fb8bd2899449",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationProductBook_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationProductBook_rootQuery(\n  $productId: String!\n) {\n  product(id: $productId) {\n    name\n    id\n  }\n  ...bookProduct_query\n}\n\nfragment bookProduct_query on Query {\n  product(id: $productId) {\n    id\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "e0f80baaa624b38b5c5ccef44fb01541";

export default node;
