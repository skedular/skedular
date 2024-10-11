/**
 * @generated SignedSource<<6cf686ef9e035bd37005922689111ef1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type CustomerOrderField = "designation" | "familyName" | "givenName" | "locale" | "middleName" | "name" | "timezone" | "title" | "%future added value";
export type LocationMemberOrderField = "familyName" | "givenName" | "membershipType" | "middleName" | "name" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationMemberOrderInput = {
  direction: OrderDirection;
  field: LocationMemberOrderField;
};
export type CustomerOrderInput = {
  direction: OrderDirection;
  field: CustomerOrderField;
};
export type locationPeopleTab_rootQuery$variables = {
  locationExists: boolean;
  locationId: string;
  locationOrganizationPeopleSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  locationPeopleSortingValues?: ReadonlyArray<LocationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type locationPeopleTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query" | "locationPeopleTab_query_organizationMembers">;
};
export type locationPeopleTab_rootQuery = {
  response: locationPeopleTab_rootQuery$data;
  variables: locationPeopleTab_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationOrganizationPeopleSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationPeopleSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v8 = {
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
},
v9 = [
  (v7/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationPeopleSortingValues"
  },
  (v8/*: any*/)
],
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "givenName",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "middleName",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "familyName",
  "storageKey": null
},
v14 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "photoUrl",
  "storageKey": null
},
v15 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v16 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v17 = {
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
v18 = {
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
},
v19 = [
  "where",
  "orderBy"
],
v20 = [
  (v7/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationOrganizationPeopleSortingValues"
  },
  (v8/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "locationPeopleTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPeopleTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationPeopleTab_query_organizationMembers"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/),
      (v4/*: any*/),
      (v3/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationPeopleTab_rootQuery",
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
          (v5/*: any*/),
          (v6/*: any*/)
        ],
        "storageKey": null
      },
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v9/*: any*/),
            "concreteType": "LocationMemberConnection",
            "kind": "LinkedField",
            "name": "paginatedLocationMembers",
            "plural": false,
            "selections": [
              (v10/*: any*/),
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
                      (v5/*: any*/),
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
                          (v6/*: any*/),
                          (v11/*: any*/),
                          (v12/*: any*/),
                          (v13/*: any*/),
                          (v14/*: any*/)
                        ],
                        "storageKey": null
                      },
                      (v15/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v16/*: any*/)
                ],
                "storageKey": null
              },
              (v17/*: any*/),
              (v18/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "filters": (v19/*: any*/),
            "handle": "connection",
            "key": "locationPeopleTab_paginatedLocationMembers",
            "kind": "LinkedHandle",
            "name": "paginatedLocationMembers"
          },
          {
            "alias": null,
            "args": (v20/*: any*/),
            "concreteType": "CustomerConnection",
            "kind": "LinkedField",
            "name": "paginatedCustomersByDefaultLocation",
            "plural": false,
            "selections": [
              (v10/*: any*/),
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
                      (v5/*: any*/),
                      (v6/*: any*/),
                      (v11/*: any*/),
                      (v12/*: any*/),
                      (v13/*: any*/),
                      (v14/*: any*/),
                      (v15/*: any*/)
                    ],
                    "storageKey": null
                  },
                  (v16/*: any*/)
                ],
                "storageKey": null
              },
              (v17/*: any*/),
              (v18/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v20/*: any*/),
            "filters": (v19/*: any*/),
            "handle": "connection",
            "key": "locationPeopleTab_paginatedCustomersByDefaultLocation",
            "kind": "LinkedHandle",
            "name": "paginatedCustomersByDefaultLocation"
          }
        ]
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
    "cacheID": "947dc718fb92754ab39faa08d6908a79",
    "id": null,
    "metadata": {},
    "name": "locationPeopleTab_rootQuery",
    "operationKind": "query",
    "text": "query locationPeopleTab_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $peopleNameSearchText: String\n  $locationPeopleSortingValues: [LocationMemberOrderInput!]\n  $locationOrganizationPeopleSortingValues: [CustomerOrderInput!]\n) {\n  ...locationPeopleTab_query\n  ...locationPeopleTab_query_organizationMembers\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment locationPeopleTab_query on Query {\n  location(id: $locationId) {\n    id\n    name\n  }\n  paginatedLocationMembers(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationPeopleSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationMemberCard_LocationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationSingleChoiceMembershipType_query\n}\n\nfragment locationPeopleTab_query_organizationMembers on Query {\n  paginatedCustomersByDefaultLocation(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationPeopleSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationSingleChoiceMembershipType_query on Query {\n  locationMemberMembershipTypes\n}\n"
  }
};
})();

(node as any).hash = "d1b53be3975a1d0f718bf672a10bd602";

export default node;
