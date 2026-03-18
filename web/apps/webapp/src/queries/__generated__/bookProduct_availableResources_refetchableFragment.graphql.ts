/**
 * @generated SignedSource<<90c349496aec34142672832f8cdb41dd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookProduct_availableResources_refetchableFragment$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  organizationCustomDomain?: string | null | undefined;
  productId?: string | null | undefined;
};
export type bookProduct_availableResources_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"bookProduct_availableResources_query">;
};
export type bookProduct_availableResources_refetchableFragment = {
  response: bookProduct_availableResources_refetchableFragment$data;
  variables: bookProduct_availableResources_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateFromToGetAvailableResources"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateUntilToGetAvailableResources"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationCustomDomain"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "productId"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = [
  (v2/*: any*/),
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookProduct_availableResources_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookProduct_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookProduct_availableResources_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "from",
                "variableName": "dateFromToGetAvailableResources"
              },
              {
                "kind": "Variable",
                "name": "organizationCustomDomain",
                "variableName": "organizationCustomDomain"
              },
              {
                "kind": "Variable",
                "name": "productId",
                "variableName": "productId"
              },
              {
                "kind": "Variable",
                "name": "until",
                "variableName": "dateUntilToGetAvailableResources"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingResourceDetails",
        "kind": "LinkedField",
        "name": "availableResources",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "concreteType": "Booking_LocationDetails",
            "kind": "LinkedField",
            "name": "location",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "uniqueId",
                "storageKey": null
              },
              (v1/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "resource",
            "plural": false,
            "selections": [
              (v2/*: any*/),
              (v1/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v3/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v3/*: any*/),
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
    "cacheID": "231b1d388828037bf4880e0405470ac1",
    "id": null,
    "metadata": {},
    "name": "bookProduct_availableResources_refetchableFragment",
    "operationKind": "query",
    "text": "query bookProduct_availableResources_refetchableFragment(\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $organizationCustomDomain: String\n  $productId: String\n) {\n  ...bookProduct_availableResources_query\n}\n\nfragment bookProduct_availableResources_query on Query {\n  availableResources(where: {organizationCustomDomain: $organizationCustomDomain, productId: $productId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    location {\n      uniqueId\n      name\n    }\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "47b346ffeda02c00fd399c5f975f275b";

export default node;
