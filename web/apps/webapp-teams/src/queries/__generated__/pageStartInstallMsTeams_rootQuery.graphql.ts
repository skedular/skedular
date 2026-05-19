/**
 * @generated SignedSource<<4af11280759fc05abaad0b33765fa235>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageStartInstallMsTeams_rootQuery$variables = Record<PropertyKey, never>;
export type pageStartInstallMsTeams_rootQuery$data = {
  readonly azureTenantAdminConsentUrl: string;
};
export type pageStartInstallMsTeams_rootQuery = {
  response: pageStartInstallMsTeams_rootQuery$data;
  variables: pageStartInstallMsTeams_rootQuery$variables;
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
    "name": "pageStartInstallMsTeams_rootQuery",
    "selections": (v0/*:: as any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageStartInstallMsTeams_rootQuery",
    "selections": (v0/*:: as any*/)
  },
  "params": {
    "cacheID": "8669c15cb024cbcb7f7ab0a776b3bfab",
    "id": null,
    "metadata": {},
    "name": "pageStartInstallMsTeams_rootQuery",
    "operationKind": "query",
    "text": "query pageStartInstallMsTeams_rootQuery {\n  azureTenantAdminConsentUrl\n}\n"
  }
};
})();

(node as any).hash = "78ad0c95e7b142930eff9cedf8ddee8d";

export default node;
