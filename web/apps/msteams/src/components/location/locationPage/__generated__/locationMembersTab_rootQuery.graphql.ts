/**
 * @generated SignedSource<<63cfc56afc25a3fed35ed065ef118888>>
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
export type locationMembersTab_rootQuery$variables = {
  locationExists: boolean;
  locationId: string;
  locationMembersSortingValues?: ReadonlyArray<LocationMemberOrderInput> | null | undefined;
  locationOrganizationMembersSortingValues?: ReadonlyArray<CustomerOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type locationMembersTab_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"locationMembersTab_paginatedCustomersByDefaultLocation_query" | "locationMembersTab_paginatedLocationMembers_query" | "locationMembersTab_query">;
};
export type locationMembersTab_rootQuery = {
  response: locationMembersTab_rootQuery$data;
  variables: locationMembersTab_rootQuery$variables;
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
  "name": "locationMembersSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationOrganizationMembersSortingValues"
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
    "variableName": "locationMembersSortingValues"
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
    "variableName": "locationOrganizationMembersSortingValues"
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
    "name": "locationMembersTab_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationMembersTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationMembersTab_paginatedLocationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locationMembersTab_paginatedCustomersByDefaultLocation_query"
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
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Operation",
    "name": "locationMembersTab_rootQuery",
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
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "locationMemberMembershipTypes",
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
            "key": "locationMembersTab_paginatedLocationMembers",
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
            "key": "locationMembersTab_paginatedCustomersByDefaultLocation",
            "kind": "LinkedHandle",
            "name": "paginatedCustomersByDefaultLocation"
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "f8bde01940196ba89a44cffd44493ed4",
    "id": null,
    "metadata": {},
    "name": "locationMembersTab_rootQuery",
    "operationKind": "query",
    "text": "query locationMembersTab_rootQuery(\n  $locationId: String!\n  $locationExists: Boolean!\n  $peopleNameSearchText: String\n  $locationMembersSortingValues: [LocationMemberOrderInput!]\n  $locationOrganizationMembersSortingValues: [CustomerOrderInput!]\n) {\n  ...locationMembersTab_query\n  ...locationMembersTab_paginatedLocationMembers_query\n  ...locationMembersTab_paginatedCustomersByDefaultLocation_query\n}\n\nfragment customerCard_CustomerDetails on CustomerDetails {\n  name\n  givenName\n  middleName\n  familyName\n  photoUrl\n}\n\nfragment locationMemberCard_LocationMemberDetails on LocationMemberDetails {\n  id\n  membershipType\n  customer {\n    name\n    givenName\n    middleName\n    familyName\n    photoUrl\n  }\n}\n\nfragment locationMembersTab_paginatedCustomersByDefaultLocation_query on Query {\n  paginatedCustomersByDefaultLocation(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationOrganizationMembersSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...customerCard_CustomerDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationMembersTab_paginatedLocationMembers_query on Query {\n  paginatedLocationMembers(first: 50, where: {locationId: $locationId, nameContains: $peopleNameSearchText}, orderBy: $locationMembersSortingValues) @include(if: $locationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationMemberCard_LocationMemberDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment locationMembersTab_query on Query {\n  location(id: $locationId) {\n    id\n    name\n  }\n  ...locationSingleChoiceMembershipType_query\n}\n\nfragment locationSingleChoiceMembershipType_query on Query {\n  locationMemberMembershipTypes\n}\n"
  }
};
})();

(node as any).hash = "6a8bab7c8fadf7ef76d009a24a297ccf";

export default node;
