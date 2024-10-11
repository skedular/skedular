/**
 * @generated SignedSource<<7a84143ef329e19d48c08a82a3a3b2eb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type organization_rootQuery$variables = {
  organizationId: string;
};
export type organization_rootQuery$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
};
export type organization_rootQuery = {
  response: organization_rootQuery$data;
  variables: organization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  }
],
v1 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "id",
        "variableName": "organizationId"
      }
    ],
    "concreteType": "OrganizationDetails",
    "kind": "LinkedField",
    "name": "organization",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "id",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "name",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "logoUrl",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canModify",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "canViewAnalytics",
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organization_rootQuery",
    "selections": (v1/*: any*/),
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organization_rootQuery",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "15f88c56e1d765961cbb92aa0bf70626",
    "id": null,
    "metadata": {},
    "name": "organization_rootQuery",
    "operationKind": "query",
    "text": "query organization_rootQuery(\n  $organizationId: String!\n) {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n    canViewAnalytics\n  }\n}\n"
  }
};
})();

(node as any).hash = "c2f5edf731ac9bc6c32199623ffd0454";

export default node;
