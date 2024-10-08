/**
 * @generated SignedSource<<d6e658bffeea8c22247daa959be893ca>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type pageHome_rootQuery$variables = Record<PropertyKey, never>;
export type pageHome_rootQuery$data = {
  readonly azureTenantOrganization: {
    readonly id: string;
  } | null | undefined;
  readonly isAzureTenantInstalled: boolean;
};
export type pageHome_rootQuery = {
  response: pageHome_rootQuery$data;
  variables: pageHome_rootQuery$variables;
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
    "name": "pageHome_rootQuery",
    "selections": (v0/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "pageHome_rootQuery",
    "selections": (v0/*: any*/)
  },
  "params": {
    "cacheID": "0be5a8c9e1becf33b501a138deae8576",
    "id": null,
    "metadata": {},
    "name": "pageHome_rootQuery",
    "operationKind": "query",
    "text": "query pageHome_rootQuery {\n  isAzureTenantInstalled\n  azureTenantOrganization {\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "a0ea47c765cbbc77f24ff4d7856bc864";

export default node;
