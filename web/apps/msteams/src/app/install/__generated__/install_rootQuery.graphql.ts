/**
 * @generated SignedSource<<bcc550e0535711510e66aa56e9e38f78>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type install_rootQuery$variables = Record<PropertyKey, never>;
export type install_rootQuery$data = {
  readonly azureTenantAdminConsentUrl: string;
};
export type install_rootQuery = {
  response: install_rootQuery$data;
  variables: install_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "azureTenantAdminConsentUrl",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "install_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "install_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "7d5d3104bcd9302dd1ec34a7a7e8d87c",
    "id": null,
    "metadata": {},
    "name": "install_rootQuery",
    "operationKind": "query",
    "text": "query install_rootQuery {\n  azureTenantAdminConsentUrl\n}\n"
  }
};
})();

(node as any).hash = "5219bfdd59bbe2b80d806b9a308c0eb3";

export default node;
