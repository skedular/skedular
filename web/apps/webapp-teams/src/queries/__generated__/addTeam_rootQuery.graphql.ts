/**
 * @generated SignedSource<<ac658a90b358232499741d77d4cbba41>>
 * @lightSyntaxTransform
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
  organizationCustomDomain: string;
  organizationMemberSelectorOrganizationMembersSortingValues?: ReadonlyArray<OrganizationMemberOrderInput> | null | undefined;
};
export type addTeam_rootQuery$data = {
  readonly me: {
    readonly id: string;
  };
  readonly organization: {
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
  "name": "organizationCustomDomain"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationMemberSelectorOrganizationMembersSortingValues"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "concreteType": "CustomerDetails",
  "kind": "LinkedField",
  "name": "me",
  "plural": false,
  "selections": [
    (v3/*:: as any*/)
  ],
  "storageKey": null
},
v5 = [
  {
    "kind": "Variable",
    "name": "customDomain",
    "variableName": "organizationCustomDomain"
  }
],
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = [
  (v3/*:: as any*/),
  (v6/*:: as any*/)
],
v8 = [
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
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v10 = {
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
      (v0/*:: as any*/),
      (v1/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "addTeam_rootQuery",
    "selections": [
      (v4/*:: as any*/),
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": (v7/*:: as any*/),
        "storageKey": null
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
      (v1/*:: as any*/),
      (v0/*:: as any*/),
      (v2/*:: as any*/)
    ],
    "kind": "Operation",
    "name": "addTeam_rootQuery",
    "selections": [
      (v4/*:: as any*/),
      {
        "alias": null,
        "args": (v5/*:: as any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v3/*:: as any*/),
          (v6/*:: as any*/),
          {
            "alias": null,
            "args": (v8/*:: as any*/),
            "concreteType": "ConnectionOfOrganizationMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              (v9/*:: as any*/),
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
                      (v3/*:: as any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v3/*:: as any*/),
                          (v6/*:: as any*/),
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
              (v10/*:: as any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v8/*:: as any*/),
            "filters": [
              "where",
              "orderBy"
            ],
            "handle": "connection",
            "key": "organizationMemberSelector_members",
            "kind": "LinkedHandle",
            "name": "members"
          }
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
                "name": "organizationCustomDomain",
                "variableName": "organizationCustomDomain"
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
          (v9/*:: as any*/),
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
                "selections": (v7/*:: as any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          (v10/*:: as any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "ee9e672d34ced9567ae1982709116952",
    "id": null,
    "metadata": {},
    "name": "addTeam_rootQuery",
    "operationKind": "query",
    "text": "query addTeam_rootQuery(\n  $organizationCustomDomain: String!\n  $bookingPeopleNameSearchText: String\n  $organizationMemberSelectorOrganizationMembersSortingValues: [OrganizationMemberOrderInput!]\n) {\n  me {\n    id\n  }\n  organization(customDomain: $organizationCustomDomain) {\n    id\n    name\n  }\n  ...organizationMemberSelector_query\n  ...singleChoiceLocation_locations_query\n}\n\nfragment organizationMemberSelector_query on Query {\n  organization(customDomain: $organizationCustomDomain) {\n    members(where: {nameContains: $bookingPeopleNameSearchText}, orderBy: $organizationMemberSelectorOrganizationMembersSortingValues) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n          }\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceLocation_locations_query on Query {\n  locations(where: {organizationCustomDomain: $organizationCustomDomain}) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e0277ec6ff7448206ce0028e2053c9d9";

export default node;
