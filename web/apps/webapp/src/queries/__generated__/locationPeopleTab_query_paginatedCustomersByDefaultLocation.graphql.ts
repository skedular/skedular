/**
 * @generated SignedSource<<e14e0af7e25230211e5b528c2ca37eab>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CustomerOrderField = "designation" | "familyName" | "givenName" | "locale" | "middleName" | "name" | "timezone" | "title" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type CustomerOrderInput = {
  direction: OrderDirection;
  field?: CustomerOrderField | null | undefined;
};
export type locationPeopleTab_query_paginatedCustomersByDefaultLocation$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationId: string;
  locationOrganizationPeopleSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type locationPeopleTab_query_paginatedCustomersByDefaultLocation$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query_organizationMembers">;
};
export type locationPeopleTab_query_paginatedCustomersByDefaultLocation = {
  response: locationPeopleTab_query_paginatedCustomersByDefaultLocation$data;
  variables: locationPeopleTab_query_paginatedCustomersByDefaultLocation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": 50,
    "kind": "LocalArgument",
    "name": "count"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "cursor"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationOrganizationPeopleSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "peopleNameSearchText"
  }
],
v1 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationOrganizationPeopleSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "locationId",
        "variableName": "locationId"
      },
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "locationPeopleTab_query_paginatedCustomersByDefaultLocation",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "locationPeopleTab_query_organizationMembers"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationPeopleTab_query_paginatedCustomersByDefaultLocation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "CustomerConnection",
        "kind": "LinkedField",
        "name": "paginatedCustomersByDefaultLocation",
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
            "concreteType": "CustomerEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "CustomerDetails",
                "kind": "LinkedField",
                "name": "node",
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
                    "name": "givenName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "middleName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "familyName",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "photoUrl",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "__typename",
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cursor",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "PageInfo",
            "kind": "LinkedField",
            "name": "pageInfo",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "endCursor",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "hasNextPage",
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
      },
      {
        "alias": null,
        "args": (v1/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locationPeopleTab_paginatedCustomersByDefaultLocation",
        "kind": "LinkedHandle",
        "name": "paginatedCustomersByDefaultLocation"
      }
    ]
  },
  "params": {
    "cacheID": "3d8e0de1c6d55cdd86436e7f052f0776",
    "id": null,
    "metadata": {},
    "name": "locationPeopleTab_query_paginatedCustomersByDefaultLocation",
    "operationKind": "query",
    "text": "query locationPeopleTab_query_paginatedCustomersByDefaultLocation(\n  $count: Int = 50\n  $cursor: String\n  $locationId: String!\n  $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]\n  $peopleNameSearchText: String\n) {\n  ...locationPeopleTab_query_organizationMembers_1G22uz\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment locationPeopleTab_query_organizationMembers_1G22uz on Query {\n  paginatedCustomersByDefaultLocation(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "3436bb42cdeff6688cfdd9e7509b9360";

export default node;
