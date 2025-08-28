/**
 * @generated SignedSource<<1d0064011d293c848121308ed2dea549>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type OrderDirection = "ASCENDING" | "DESCENDING" | "%future added value";
export type OrganizationMemberOrderField = "FAMILY_NAME" | "GIVEN_NAME" | "MIDDLE_NAME" | "NAME" | "PHONE_NUMBER" | "ROLE" | "STATUS" | "%future added value";
export type OrganizationMemberOrderInput = {
  direction: OrderDirection;
  field: OrganizationMemberOrderField;
};
export type addTeam_rootQuery$variables = {
  bookingPeopleNameSearchText?: string | null | undefined;
  organizationExists: boolean;
  organizationMemberSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
  organizationUniqueAlphanumericName: string;
};
export type addTeam_rootQuery$data = {
  readonly me: {
    readonly id: string;
  };
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMemberSelector_query" | "singleChoiceLocation_locations_query">;
};
export type addTeam_rootQuery = {
  response: addTeam_rootQuery$data;
  variables: addTeam_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "bookingPeopleNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMemberSelectorOrganizationMembersSortingValues"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v4/*: any*/)
  ],
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = [
  (v4/*: any*/),
  (v6/*: any*/)
],
v8 = {
  "alias": null,
  "args": [
    {
      "kind": "Variable",
      "name": "uniqueAlphanumericName",
      "variableName": "organizationUniqueAlphanumericName"
    }
  ],
  "concreteType": "OrganizationDetails",
  "kind": "LinkedField",
  "name": "organization",
  "plural": false,
  "selections": (v7/*: any*/),
  "storageKey": null
},
v9 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v10 = [
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationMemberSelectorOrganizationMembersSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "bookingPeopleNameSearchText"
      },
      (v9/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v12 = {
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
};
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addTeam_rootQuery",
    "selections": [
      (v5/*: any*/),
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          (v8/*: any*/)
        ]
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMemberSelector_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "singleChoiceLocation_locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v3/*: any*/),
      (v1/*: any*/),
      (v0/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "addTeam_rootQuery",
    "selections": [
      (v5/*: any*/),
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          (v8/*: any*/),
          {
            "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
            "args": (v10/*: any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "organizationMembers",
            "plural": false,
            "selections": [
              (v11/*: any*/),
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
                      (v4/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Organization_CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "uniqueId",
                            "storageKey": null
                          },
                          (v6/*: any*/),
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
              (v12/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": "organizationMemberSelectorPaginatedOrganizationMembers",
            "args": (v10/*: any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "organizationMemberSelector_organizationMemberSelectorPaginatedOrganizationMembers",
            "kind": "LinkedHandle",
            "name": "organizationMembers"
          },
          {
            "alias": null,
            "args": [
              {
                "fields": [
                  (v9/*: any*/)
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
              (v11/*: any*/),
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
                    "selections": (v7/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v12/*: any*/)
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "6c93391f2257a30f8d2e2b6d206f9d66",
    "id": null,
    "metadata": {},
    "name": "addTeam_rootQuery",
    "operationKind": "query",
    "text": "query addTeam_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $organizationExists: Boolean!\n  $bookingPeopleNameSearchText: String\n  $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n) {\n  me {\n    id\n  }\n  organization(uniqueAlphanumericName: $organizationUniqueAlphanumericName) @include(if: $organizationExists) {\n    id\n    name\n  }\n  ...organizationMemberSelector_query\n  ...singleChoiceLocation_locations_query\n}\n\nfragment organizationMemberSelector_query on Query {\n  organizationMemberSelectorPaginatedOrganizationMembers: organizationMembers(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName, nameContains: $bookingPeopleNameSearchText}, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n}\n\nfragment singleChoiceLocation_locations_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "414317bce4ad8100e25177ec4ac9b457";

export default node;
