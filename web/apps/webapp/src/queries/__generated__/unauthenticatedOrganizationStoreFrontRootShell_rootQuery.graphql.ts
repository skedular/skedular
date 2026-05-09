/**
 * @generated SignedSource<<58a69515ac567e6591f883c21dfd72f7>>
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
  readonly organizationPublic: {
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
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
],
v1 = {
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
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "logoUrl",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "selections": [
      (v1/*: any*/),
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
      (v1/*: any*/)
    ]
  },
  "params": {
    "cacheID": "917f8d71973231fc09265af8bee163dd",
    "id": null,
    "metadata": {},
    "name": "unauthenticatedOrganizationStoreFrontRootShell_rootQuery",
    "operationKind": "query",
    "text": "query unauthenticatedOrganizationStoreFrontRootShell_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n    logoUrl\n  }\n  ...unauthenticatedOrganizationStoreFrontAppBar_query\n}\n\nfragment unauthenticatedOrganizationStoreFrontAppBar_query on Query {\n  organizationPublic(customDomain: $organizationCustomDomain) {\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "0138138abf482725d166074623a74e4e";

export default node;
