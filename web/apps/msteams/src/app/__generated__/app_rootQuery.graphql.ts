/**
 * @generated SignedSource<<f7af86cf20f0480752caa0c7627c2dad>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
export type app_rootQuery$variables = Record<PropertyKey, never>;
export type app_rootQuery$data = {
  readonly azureTenantOrganization: {
    readonly id: string;
  } | null | undefined;
  readonly isAzureTenantInstalled: boolean;
};
export type app_rootQuery = {
  response: app_rootQuery$data;
  variables: app_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "isAzureTenantInstalled",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "azureTenantOrganization",
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
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "app_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "app_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "57ab42688324914175105321b450bed2",
    "id": null,
    "metadata": {},
    "name": "app_rootQuery",
    "operationKind": "query",
    "text": "query app_rootQuery {\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "543a01ea8c48af013cfa0fa4362113e0";

export default node;
