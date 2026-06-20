/**
 * @generated SignedSource<<3c4383bc2975aa02d87ed8483ce3c18b>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type hostOperationsOrganizationQuery$variables = {
  customDomain: string;
};
export type hostOperationsOrganizationQuery$data = {
  readonly organization: {
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly customDomain: string | null | undefined;
    readonly isOwnershipVerified: boolean;
    readonly marketplaceListingMetadata: {
      readonly about: string | null | undefined;
    };
    readonly name: string;
    readonly website: string | null | undefined;
  } | null | undefined;
};
export type hostOperationsOrganizationQuery = {
  response: hostOperationsOrganizationQuery$data;
  variables: hostOperationsOrganizationQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customDomain"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "customDomain"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "customDomain",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "website",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "contactEmail",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "contactPhone",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "isOwnershipVerified",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "marketplaceListingMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "about",
      "storageKey": null
    }
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "hostOperationsOrganizationQuery",
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
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/)
        ],
        "storageKey": null
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*:: as any*/),
    "kind": "Operation",
    "name": "hostOperationsOrganizationQuery",
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
          (v3/*:: as any*/),
          (v4/*:: as any*/),
          (v5/*:: as any*/),
          (v6/*:: as any*/),
          (v7/*:: as any*/),
          (v8/*:: as any*/),
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
    ]
  },
  "params": {
    "cacheID": "0b1f693419f614b0c17c7a9e334db637",
    "id": null,
    "metadata": {},
    "name": "hostOperationsOrganizationQuery",
    "operationKind": "query",
    "text": "query hostOperationsOrganizationQuery(\n  $customDomain: String!\n) {\n  organization(customDomain: $customDomain) {\n    name\n    customDomain\n    website\n    contactEmail\n    contactPhone\n    isOwnershipVerified\n    marketplaceListingMetadata {\n      about\n    }\n    id\n  }\n}\n"
  }
};
})();

(node as any).hash = "3f4b73391e2e98ad6509afadc93d9d84";

export default node;
