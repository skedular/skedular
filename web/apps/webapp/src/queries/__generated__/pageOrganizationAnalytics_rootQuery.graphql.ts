/**
 * @generated SignedSource<<b149ecc32a4a15af3a7afdcd61364f8c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "ABOUT" | "NAME" | "TIMEZONE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type pageOrganizationAnalytics_rootQuery$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type pageOrganizationAnalytics_rootQuery$data = {
  readonly organization: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAnalytics_query">;
};
export type pageOrganizationAnalytics_rootQuery = {
  response: pageOrganizationAnalytics_rootQuery$data;
  variables: pageOrganizationAnalytics_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v2 = [
  {
    "kind": "Variable",
    "name": "uniqueAlphanumericName",
    "variableName": "organizationUniqueAlphanumericName"
  }
],
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageOrganizationAnalytics_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationAnalytics_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationAnalytics_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v2/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          (v4/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "orderBy",
            "variableName": "locationsSortingValues"
          },
          {
            "fields": [
              {
                "kind": "Variable",
                "name": "organizationUniqueAlphanumericName",
                "variableName": "organizationUniqueAlphanumericName"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "ConnectionOfLocationEdge",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "totalCount",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v4/*: any*/),
                  (v3/*: any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "kind": "ClientExtension",
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "__id",
                "storageKey": null
              }
            ]
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "70d4b3792ba578cb3329570bff96e10b",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationAnalytics_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationAnalytics_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) {\n    name\n    id\n  }\n  ...organizationAnalytics_query\n}\n\nfragment locationSelector_allLocations_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n\nfragment organizationAnalytics_query on Query {\n  ...locationSelector_allLocations_query\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "447984e4226b2551cd7b8e51a70ffffa";

export default node;
