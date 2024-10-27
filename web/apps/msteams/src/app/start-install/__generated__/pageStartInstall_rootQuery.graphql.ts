/**
 * @generated SignedSource<<60b48b17b2adc3710b6f75ea463ec52c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageStartInstall_rootQuery$variables = Record<PropertyKey, never>;
export type pageStartInstall_rootQuery$data = {
  readonly azureTenantAdminConsentUrl: string;
};
export type pageStartInstall_rootQuery = {
  response: pageStartInstall_rootQuery$data;
  variables: pageStartInstall_rootQuery$variables;
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
    "name": "pageStartInstall_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageStartInstall_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "d43b0dc58d7ff8796fe61f8a3b086212",
    "id": null,
    "metadata": {},
    "name": "pageStartInstall_rootQuery",
    "operationKind": "query",
    "text": "query pageStartInstall_rootQuery {\n  azureTenantAdminConsentUrl\n}\n"
  }
};
})();

(node as any).hash = "7a67abfc16d512bd7f522039bf9a5fc7";

export default node;
