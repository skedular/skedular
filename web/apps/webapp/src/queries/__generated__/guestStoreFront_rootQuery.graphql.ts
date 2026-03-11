/**
 * @generated SignedSource<<f8d6e0fcdb197f0fb62a742822aa4c68>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type guestStoreFront_rootQuery$variables = {
  organizationUniqueAlphanumericName: string;
};
export type guestStoreFront_rootQuery$data = {
  readonly organizationPublic: {
    readonly listingMetadata: {
      readonly mainHeader: string;
      readonly subHeader: string;
    };
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"guestStoreFrontFooter_query">;
};
export type guestStoreFront_rootQuery = {
  response: guestStoreFront_rootQuery$data;
  variables: guestStoreFront_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationUniqueAlphanumericName"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v2 = {
  "alias": null,
  "args": null,
  "concreteType": "ListingMetadata",
  "kind": "LinkedField",
  "name": "listingMetadata",
  "plural": false,
  "selections": [
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "mainHeader",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "subHeader",
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
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "guestStoreFrontFooter_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "guestStoreFront_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "OrganizationPublicDetails",
        "kind": "LinkedField",
        "name": "organizationPublic",
        "plural": false,
        "selections": [
          (v2/*: any*/),
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
            "name": "contactPhone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "contactEmail",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationPhysicalAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine1",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "addressLine2",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "suburb",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "city",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "province",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "zipcode",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "country",
                "storageKey": null
              },
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
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "00bfcfea21c051ae9b6d7baa2476f010",
    "id": null,
    "metadata": {},
    "name": "guestStoreFront_rootQuery",
    "operationKind": "query",
    "text": "query guestStoreFront_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n) {\n  organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    listingMetadata {\n      mainHeader\n      subHeader\n    }\n  }\n  ...guestStoreFrontFooter_query\n}\n\nfragment guestStoreFrontFooter_query on Query {\n  organizationPublic(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n    contactPhone\n    contactEmail\n    physicalAddress {\n      addressLine1\n      addressLine2\n      suburb\n      city\n      province\n      zipcode\n      country\n      id\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "251e548f906e0d02725b1751d35f29fe";

export default node;
