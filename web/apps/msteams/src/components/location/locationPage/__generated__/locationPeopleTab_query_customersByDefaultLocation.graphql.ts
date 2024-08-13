/**
 * @generated SignedSource<<95603b1bb097de19a86a3c90e99daead>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CustomerOrderField = "designation" | "familyName" | "givenName" | "locale" | "middleName" | "name" | "timezone" | "title" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type CustomerOrderInput = {
  direction: OrderDirection;
  field?: CustomerOrderField | null | undefined;
};
export type locationPeopleTab_query_customersByDefaultLocation$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationId: string;
  locationOrganizationPeopleSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type locationPeopleTab_query_customersByDefaultLocation$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query_organizationMembers">;
};
export type locationPeopleTab_query_customersByDefaultLocation = {
  response: locationPeopleTab_query_customersByDefaultLocation$data;
  variables: locationPeopleTab_query_customersByDefaultLocation$variables;
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
    "name": "locationPeopleTab_query_customersByDefaultLocation",
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
    "name": "locationPeopleTab_query_customersByDefaultLocation",
    "selections": [
      {
        "alias": null,
        "args": (v1/*: any*/),
        "concreteType": "CustomerConnection",
        "kind": "LinkedField",
        "name": "customersByDefaultLocation",
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
        "key": "locationPeopleTab_customersByDefaultLocation",
        "kind": "LinkedHandle",
        "name": "customersByDefaultLocation"
      }
    ]
  },
  "params": {
    "cacheID": "bc525c9438e89c60b977ab00ec3fb170",
    "id": null,
    "metadata": {},
    "name": "locationPeopleTab_query_customersByDefaultLocation",
    "operationKind": "query",
    "text": "query locationPeopleTab_query_customersByDefaultLocation(\n  $count: Int = 50\n  $cursor: String\n  $locationId: String!\n  $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]\n  $peopleNameSearchText: String\n) {\n  ...locationPeopleTab_query_organizationMembers_1G22uz\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment locationPeopleTab_query_organizationMembers_1G22uz on Query {\n  customersByDefaultLocation(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "96b6bc1b055fe904a32f7df1ef086f01";

export default node;
