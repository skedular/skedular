/**
 * @generated SignedSource<<f7e830928fff9966736bc22bc615b043>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type pageOrganizationTeam_rootQuery$variables = {
  organizationExists: boolean;
  organizationUniqueAlphanumericName: string;
  peopleNameSearchText?: string | null | undefined;
  teamId: string;
};
export type pageOrganizationTeam_rootQuery$data = {
  readonly team: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeam_query" | "organizationTeam_teamMembers_query">;
};
export type pageOrganizationTeam_rootQuery = {
  response: pageOrganizationTeam_rootQuery$data;
  variables: pageOrganizationTeam_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationUniqueAlphanumericName"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "peopleNameSearchText"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamId"
},
v4 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "teamId"
  }
],
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v7 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
],
v8 = [
  (v6/*: any*/),
  (v5/*: any*/)
],
v9 = [
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
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v11 = {
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
    "name": "pageOrganizationTeam_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v5/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTeam_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTeam_teamMembers_query"
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
      (v3/*: any*/),
      (v2/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageOrganizationTeam_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
        "concreteType": "TeamDetails",
        "kind": "LinkedField",
        "name": "team",
        "plural": false,
        "selections": [
          (v5/*: any*/),
          (v6/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "about",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "timezone",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "CdnImageFile",
            "kind": "LinkedField",
            "name": "featureImages",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "original",
                "plural": false,
                "selections": (v7/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "CdnFile",
                "kind": "LinkedField",
                "name": "thumbnail",
                "plural": false,
                "selections": (v7/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "LocationDetails",
            "kind": "LinkedField",
            "name": "primaryLocation",
            "plural": false,
            "selections": (v8/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "concreteType": "ConnectionOfTeamMemberEdge",
            "kind": "LinkedField",
            "name": "members",
            "plural": false,
            "selections": [
              (v10/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamMemberEdge",
                "kind": "LinkedField",
                "name": "edges",
                "plural": true,
                "selections": [
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamMemberDetails",
                    "kind": "LinkedField",
                    "name": "node",
                    "plural": false,
                    "selections": [
                      (v6/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "CustomerDetails",
                        "kind": "LinkedField",
                        "name": "customer",
                        "plural": false,
                        "selections": [
                          (v6/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "email",
                            "storageKey": null
                          },
                          (v5/*: any*/),
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
                            "name": "phoneNumber",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "status",
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "kind": "ScalarField",
                        "name": "role",
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
              (v11/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": (v9/*: any*/),
            "filters": [
              "where"
            ],
            "handle": "connection",
            "key": "teamMembers_members",
            "kind": "LinkedHandle",
            "name": "members"
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "kind": "ScalarField",
        "name": "teamMemberRoles",
        "storageKey": null
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
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
              (v10/*: any*/),
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
                    "selections": (v8/*: any*/),
                    "storageKey": null
                  }
                ],
                "storageKey": null
              },
              (v11/*: any*/)
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "4296f9995d5706576147970fba036483",
    "id": null,
    "metadata": {},
    "name": "pageOrganizationTeam_rootQuery",
    "operationKind": "query",
    "text": "query pageOrganizationTeam_rootQuery(\n  $organizationUniqueAlphanumericName: String!\n  $organizationExists: Boolean!\n  $teamId: String!\n  $peopleNameSearchText: String\n) {\n  team(id: $teamId) {\n    name\n    id\n  }\n  ...organizationTeam_query\n  ...organizationTeam_teamMembers_query\n}\n\nfragment organizationTeam_query on Query {\n  team(id: $teamId) {\n    id\n    name\n    about\n    timezone\n    featureImages {\n      original {\n        url\n        height\n        width\n      }\n      thumbnail {\n        url\n        height\n        width\n      }\n    }\n    primaryLocation {\n      id\n      name\n    }\n  }\n  teamMemberRoles\n  ...singleChoiceLocation_locations_query\n}\n\nfragment organizationTeam_teamMembers_query on Query {\n  team(id: $teamId) {\n    members(where: {nameContains: $peopleNameSearchText}) {\n      totalCount\n      edges {\n        node {\n          id\n          customer {\n            id\n            email\n            name\n            givenName\n            middleName\n            familyName\n            photoUrl\n            phoneNumber\n          }\n          status\n          role\n          __typename\n        }\n        cursor\n      }\n      pageInfo {\n        endCursor\n        hasNextPage\n      }\n    }\n    id\n  }\n}\n\nfragment singleChoiceLocation_locations_query on Query {\n  locations(where: {organizationUniqueAlphanumericName: $organizationUniqueAlphanumericName}) @include(if: $organizationExists) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9e0208fd7ae3ec866273c85eb350fd80";

export default node;
