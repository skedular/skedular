/**
 * @generated SignedSource<<a0bc846c57dd2cbd9256d8081029c1e9>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageInstall_rootQuery$variables = Record<PropertyKey, never>;
export type pageInstall_rootQuery$data = {
  readonly azureTenantAdminConsentUrl: string;
};
export type pageInstall_rootQuery = {
  response: pageInstall_rootQuery$data;
  variables: pageInstall_rootQuery$variables;
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
    "name": "pageInstall_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageInstall_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "4e055d8d5b79bf124bbf88ed428add62",
    "id": null,
    "metadata": {},
    "name": "pageInstall_rootQuery",
    "operationKind": "query",
    "text": "query pageInstall_rootQuery {\n  azureTenantAdminConsentUrl\n}\n"
  }
};
})();

(node as any).hash = "5e7bd8dc700ddc97ad2781f6d7b38c9d";

export default node;
