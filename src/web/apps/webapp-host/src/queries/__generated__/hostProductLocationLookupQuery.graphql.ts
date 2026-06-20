/**
 * @generated SignedSource<<b68db171a0c40ea053844f30dc05db47>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type hostProductLocationLookupQuery$variables = {
  productId: string;
};
export type hostProductLocationLookupQuery$data = {
  readonly product: {
    readonly id: string;
  } | null | undefined;
};
export type hostProductLocationLookupQuery = {
  response: hostProductLocationLookupQuery$data;
  variables: hostProductLocationLookupQuery$variables;
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
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "productId"
      }
    ],
    "concreteType": "ProductDetails",
    "kind": "LinkedField",
    "name": "product",
    "plural": false,
    "selections": [
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "hostProductLocationLookupQuery",
    "selections": (v1/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "hostProductLocationLookupQuery",
    "selections": (v1/*:: as any*/)
  },
  "params": {
    "cacheID": "3692572a12a35a9d7778c34abb4c0f28",
    "id": null,
    "metadata": {},
    "name": "hostProductLocationLookupQuery",
    "operationKind": "query",
    "text": "query hostProductLocationLookupQuery(\n  $productId: String!\n) {\n  product(id: $productId) {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "1fa48b47573f55d0a4316805f6d50106";

export default node;
