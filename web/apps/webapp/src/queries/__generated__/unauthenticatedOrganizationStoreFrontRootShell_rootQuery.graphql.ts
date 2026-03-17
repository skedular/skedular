/**
 * @generated SignedSource<<d8efa20495f5ee8e9721013ca1a7fc43>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type unauthenticatedOrganizationStoreFrontRootShell_rootQuery$variables = {
  organizationCustomDomain: string;
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
    "name": "organizationCustomDomain"
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
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
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
    "cacheID": "82a6ec3626673e89581e79bb5196fbdd",
    "id": null,
    "metadata": {},
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "operationKind": "query",
    "text": "query unauthenticatedOrganizationStoreFrontRootShell_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  ...unauthenticatedOrganizationStoreFrontAppBar_query\n}\n\nfragment unauthenticatedOrganizationStoreFrontAppBar_query on Query {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "59af04bd525cb1eb4e52c52aaafd283f";

export default node;
