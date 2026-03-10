/**
 * @generated SignedSource<<5f2d360ca12d58b4871394ea7ef08f66>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type unauthenticatedOrganizationStoreFrontRootShell_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
};
export type unauthenticatedOrganizationStoreFrontRootShell_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"unauthenticatedOrganizationStoreFrontAppBar_query">;
};
export type unauthenticatedOrganizationStoreFrontRootShell_rootQuery = {
  response: unauthenticatedOrganizationStoreFrontRootShell_rootQuery$data;
  variables: unauthenticatedOrganizationStoreFrontRootShell_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "unauthenticatedOrganizationStoreFrontAppBar_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "uniqueAlphanumericName",
            "variableName": "organizationUniqueAlphanumericName"
          }
        ],
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2e473cdc1e2b78cde1ff19e38551ff9a",
    "id": null,
    "metadata": {},
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "operationKind": "query",
    "text": "query unauthenticatedOrganizationStoreFrontRootShell_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n) {\n  ...unauthenticatedOrganizationStoreFrontAppBar_query\n}\n\nfragment unauthenticatedOrganizationStoreFrontAppBar_query on Query {\n  organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "ce8f5220f713a3f5828d453680012feb";

export default node;
