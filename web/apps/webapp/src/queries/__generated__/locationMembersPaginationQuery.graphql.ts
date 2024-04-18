/**
 * @generated SignedSource<<a9f84f9093d34a89c1567f1897b29e33>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Query } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationMemberOrderInput = {
  direction: OrderDirection;
  field?: LocationMemberOrderField | null | undefined;
};
export type locationMembersPaginationQuery$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  locationId: string;
  locationPeopleSortingValues?: ReadonlyArray<LocationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type locationMembersPaginationQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query">;
};
export type locationMembersPaginationQuery = {
  response: locationMembersPaginationQuery$data;
  variables: locationMembersPaginationQuery$variables;
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
    "name": "locationPeopleSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "peopleNameSearchText"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
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
    "variableName": "locationPeopleSortingValues"
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
    "name": "locationMembersPaginationQuery",
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
        "name": "locationPeopleTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "locationMembersPaginationQuery",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "id",
            "variableName": "locationId"
          }
        ],
        "concreteType": "LocationDetails",
        "kind": "LinkedField",
        "name": "location",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          (v2/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v3/*: any*/),
        "concreteType": "LocationMemberConnection",
        "kind": "LinkedField",
        "name": "paginatedLocationMembers",
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
            "concreteType": "LocationMemberEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "LocationMemberDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v1/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "membershipType",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v2/*: any*/),
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
                      }
                    ],
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
        "args": (v3/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locationPeopleTab_paginatedLocationMembers",
        "kind": "LinkedHandle",
        "name": "paginatedLocationMembers"
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "locationMemberMembershipTypes",
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "2df6f99c2d0b276d6ca98b84bba50559",
    "id": null,
    "metadata": {},
    "name": "locationMembersPaginationQuery",
    "operationKind": "query",
    "text": "query locationMembersPaginationQuery(\n  $count: Int = 50\n  $cursor: String\n  $locationId: String!\n  $locationPeopleSortingValues: [LocationMemberOrderInput!]\n  $peopleNameSearchText: String\n) {\n  ...locationPeopleTab_query_1G22uz\n}\n\nfragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment locationPeopleTab_query_1G22uz on Query {\n  location(id: $locationId) {\n    id\n    name\n  }\n  paginatedLocationMembers(first: $count, after: $cursor, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationPeopleSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationMemberCard_LocationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationSingleChoiceMembershipType_query\n}\n\nfragment locationSingleChoiceMembershipType_query on Query {\n  locationMemberMembershipTypes\n}\n"
  }
};
})();

(node as any).hash = "6fc770e30ecd5ffb3cfae2d781dc59e7";

export default node;
