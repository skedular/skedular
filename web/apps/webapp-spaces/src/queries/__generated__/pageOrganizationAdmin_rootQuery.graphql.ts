/**
 * @generated SignedSource<<47026e5acf0a398736163275634e5846>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationAdmin_rootQuery$variables = {
  organizationCustomDomain: string;
};
export type pageOrganizationAdmin_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_query">;
};
export type pageOrganizationAdmin_rootQuery = {
  response: pageOrganizationAdmin_rootQuery$data;
  variables: pageOrganizationAdmin_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "title",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAdmin_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "pageOrganizationAdmin_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v2/*:: as any*/),
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
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": (v3/*:: as any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "marketplaceListingMetadata",
            "plural": false,
            "selections": (v3/*:: as any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2a2d8e298fea8a3e798250405ce7d351",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationAdmin_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationAdmin_rootQuery(\n  $organizationCustomDomain: String!\n) {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    id\n  }\n  ...organizationAdmin_query\n}\n\nfragment organizationAdmin_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    name\n    listingMetadata {\n      title\n    }\n    marketplaceListingMetadata {\n      title\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "63329a28d18b7bfcea560e7999370945";

export default node;
