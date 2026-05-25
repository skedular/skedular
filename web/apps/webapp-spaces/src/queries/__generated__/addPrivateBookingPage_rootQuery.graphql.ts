/**
 * @generated SignedSource<<e2f1a8d4aec852932543b558e8eba5c8>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "NAME" | "TIMEZONE" | "TYPE" | "%future added value";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type addPrivateBookingPage_rootQuery$variables = {
  dateFromToGetAvailableResources: any;
  dateUntilToGetAvailableResources: any;
  locationId: string;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationCustomDomain: string;
  organizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  peopleNameSearchText?: string | null | undefined;
};
export type addPrivateBookingPage_rootQuery$data = {
  readonly " $fragmentSpreads": FragmentRefs<"addPrivateBookingPage_availableResources_query" | "addPrivateBookingPage_organizationMembers_query" | "addPrivateBookingPage_query">;
};
export type addPrivateBookingPage_rootQuery = {
  response: addPrivateBookingPage_rootQuery$data;
  variables: addPrivateBookingPage_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateFromToGetAvailableResources"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "dateUntilToGetAvailableResources"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationsSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationCustomDomain"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMembersSortingValues"
},
v6 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "kind": "Variable",
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v10 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "peopleNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v11 = [
  (v7/*:: as any*/),
  (v9/*:: as any*/),
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
    "argumentDefinitions": [
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/),
      (v3/*:: as any*/),
      (v4/*:: as any*/),
      (v5/*:: as any*/),
      (v6/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addPrivateBookingPage_rootQuery",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addPrivateBookingPage_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addPrivateBookingPage_organizationMembers_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "addPrivateBookingPage_availableResources_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v4/*:: as any*/),
      (v6/*:: as any*/),
      (v2/*:: as any*/),
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v5/*:: as any*/),
      (v3/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "addPrivateBookingPage_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v7/*:: as any*/)
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
              (v8/*:: as any*/)
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
                  (v7/*:: as any*/),
                  (v9/*:: as any*/)
                ],
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "bookingSlotSizeInMinutes",
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "BookingCategoryDetails",
        "kind": "LinkedField",
        "name": "bookingCategories",
        "plural": true,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "category",
            "storageKey": null
          },
          (v9/*:: as any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": [
          {
            "kind": "Variable",
            "name": "customDomain",
            "variableName": "organizationCustomDomain"
          }
        ],
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": (v10/*:: as any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "OrganizationMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v7/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v7/*:: as any*/),
                          (v9/*:: as any*/),
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
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v10/*:: as any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "addPrivateBookingPage_members",
            "kind": "LinkedHandle",
            "name": "members"
          },
          (v7/*:: as any*/)
        ],
        "storageKey": null
      },
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
                "name": "locationId",
                "variableName": "locationId"
              },
              (v8/*:: as any*/),
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
            "concreteType": "ResourceDetails",
            "kind": "LinkedField",
            "name": "resource",
            "plural": false,
            "selections": [
              (v7/*:: as any*/),
              (v9/*:: as any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v11/*:: as any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "OrganizationTagDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v11/*:: as any*/),
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
    "cacheID": "229729328e7aaa6c3f2d1a85005eb64c",
    "id": null,
    "metadata": {},
    "name": "addPrivateBookingPage_rootQuery",
    "operationKind": "query",
    "text": "query addPrivateBookingPage_rootQuery(\n  $organizationCustomDomain: String!\n  $peopleNameSearchText: String\n  $locationId: String!\n  $dateFromToGetAvailableResources: DateTime!\n  $dateUntilToGetAvailableResources: DateTime!\n  $organizationMembersSortingValues: [OrganizationMemberOrderInput!]\n  $locationsSortingValues: [LocationOrderInput!]\n) {\n  ...addPrivateBookingPage_query\n  ...addPrivateBookingPage_organizationMembers_query\n  ...addPrivateBookingPage_availableResources_query\n}\n\nfragment addPrivateBookingPage_availableResources_query on Query {\n  availableResources(where: {organizationCustomDomain: $organizationCustomDomain, locationId: $locationId, from: $dateFromToGetAvailableResources, until: $dateUntilToGetAvailableResources}) {\n    resource {\n      id\n      name\n      customTags {\n        id\n        name\n        color\n      }\n      zones {\n        id\n        name\n        color\n      }\n    }\n  }\n}\n\nfragment addPrivateBookingPage_organizationMembers_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $peopleNameSearchText}, orderBy: $organizationMembersSortingValues) {\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment addPrivateBookingPage_query on Query {\n  me {\n    id\n  }\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}, orderBy: $locationsSortingValues) {\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  bookingSlotSizeInMinutes\n  ...singleChoiceBookingCategory_query\n}\n\nfragment singleChoiceBookingCategory_query on Query {\n  bookingCategories {\n    category\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "80c77b859af885bb6da65ee97d470ac9";

export default node;
