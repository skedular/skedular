/**
 * @generated SignedSource<<237d8e524c7736a57500c9d2c648170d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "name" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type TeamOrderField = "about" | "name" | "website" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type TeamOrderInput = {
  direction: OrderDirection;
  field: TeamOrderField;
};
export type organization_rootQuery$variables = {
  locationNameSearchText?: string | null | undefined;
  organizationExists: boolean;
  organizationId: string;
  organizationLocationsSortingValues: ReadonlyArray<LocationOrderInput>;
  organizationTeamsSortingValues: ReadonlyArray<TeamOrderInput>;
  teamNameSearchText?: string | null | undefined;
};
export type organization_rootQuery$data = {
  readonly organization: {
    readonly canModify: boolean;
    readonly canViewAnalytics: boolean;
    readonly id: string;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationBillingTab_query" | "organizationLocationsTab_query" | "organizationOfferingTab_query" | "organizationTeamsTab_query">;
};
export type organization_rootQuery = {
  response: organization_rootQuery$data;
  variables: organization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
var v0 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "locationNameSearchText"
},
v1 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationExists"
},
v2 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationId"
},
v3 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationLocationsSortingValues"
},
v4 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "organizationTeamsSortingValues"
},
v5 = {
  "defaultValue": null,
  "kind": "LocalArgument",
  "name": "teamNameSearchText"
},
v6 = [
  {
    "kind": "Variable",
    "name": "id",
    "variableName": "organizationId"
  }
],
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v8 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v9 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "logoUrl",
  "storageKey": null
},
v10 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canModify",
  "storageKey": null
},
v11 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "canViewAnalytics",
  "storageKey": null
},
v12 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "unitPrice",
  "storageKey": null
},
v13 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationFeatureSetDetails",
  "kind": "LinkedField",
  "name": "featureSet",
  "plural": true,
  "selections": [
    (v8/*: any*/),
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "description",
      "storageKey": null
    }
  ],
  "storageKey": null
},
v14 = {
  "kind": "Literal",
  "name": "first",
  "value": 50
},
v15 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v16 = [
  (v14/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationLocationsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationNameSearchText"
      },
      (v15/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v17 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "totalCount",
  "storageKey": null
},
v18 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v8/*: any*/)
],
v19 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "__typename",
  "storageKey": null
},
v20 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "cursor",
  "storageKey": null
},
v21 = {
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
v22 = {
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
v23 = [
  "where",
  "orderBy"
],
v24 = [
  (v14/*: any*/),
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "organizationTeamsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "teamNameSearchText"
      },
      (v15/*: any*/)
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
],
v25 = [
  (v15/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/),
      (v2/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v11/*: any*/)
        ],
        "storageKey": null
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationLocationsTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTeamsTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationBillingTab_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationOfferingTab_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v2/*: any*/),
      (v1/*: any*/),
      (v3/*: any*/),
      (v4/*: any*/),
      (v0/*: any*/),
      (v5/*: any*/)
    ],
    "kind": "Operation",
    "name": "organization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v7/*: any*/),
          (v8/*: any*/),
          (v9/*: any*/),
          (v10/*: any*/),
          (v11/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationOfferingDetails",
            "kind": "LinkedField",
            "name": "offering",
            "plural": false,
            "selections": [
              (v7/*: any*/),
              (v8/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "start",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "end",
                "storageKey": null
              },
              (v12/*: any*/),
              (v13/*: any*/)
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "hasAttachedPaymentMethod",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationAvailableOfferingDetails",
            "kind": "LinkedField",
            "name": "availableOfferings",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "code",
                "storageKey": null
              },
              (v8/*: any*/),
              (v12/*: any*/),
              (v13/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v16/*: any*/),
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
        "plural": false,
        "selections": [
          (v17/*: any*/),
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
                  (v7/*: any*/),
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "LocationOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v18/*: any*/),
                    "storageKey": null
                  },
                  (v19/*: any*/)
                ],
                "storageKey": null
              },
              (v20/*: any*/)
            ],
            "storageKey": null
          },
          (v21/*: any*/),
          (v22/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v16/*: any*/),
        "filters": (v23/*: any*/),
        "handle": "connection",
        "key": "organizationLocationsTab_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": (v24/*: any*/),
        "concreteType": "TeamConnection",
        "kind": "LinkedField",
        "name": "teams",
        "plural": false,
        "selections": [
          (v17/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "TeamEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "TeamDetails",
                "kind": "LinkedField",
                "name": "node",
                "plural": false,
                "selections": [
                  (v7/*: any*/),
                  (v8/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "TeamOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": (v18/*: any*/),
                    "storageKey": null
                  },
                  (v19/*: any*/)
                ],
                "storageKey": null
              },
              (v20/*: any*/)
            ],
            "storageKey": null
          },
          (v21/*: any*/),
          (v22/*: any*/)
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v24/*: any*/),
        "filters": (v23/*: any*/),
        "handle": "connection",
        "key": "organizationTeamsTab_teams",
        "kind": "LinkedHandle",
        "name": "teams"
      },
      {
        "condition": "organizationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": (v25/*: any*/),
            "concreteType": "OrganizationPaymentMethod",
            "kind": "LinkedField",
            "name": "organizationPaymentMethodsDetails",
            "plural": true,
            "selections": [
              (v7/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cardBrand",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cardExpiryMonth",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cardExpiryYear",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "cardLastFourDigit",
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ]
      },
      {
        "alias": null,
        "args": (v25/*: any*/),
        "concreteType": "OrganizationBillingInfo",
        "kind": "LinkedField",
        "name": "organizationBillingInfo",
        "plural": false,
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "organizationId",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "email",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine1",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "addressLine2",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "suburb",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "city",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "province",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "zipcode",
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "country",
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "cd0aa683922acefb4d85176c34d9bfa2",
    "id": null,
    "metadata": {},
    "name": "organization_rootQuery",
    "operationKind": "query",
    "text": "query organization_rootQuery(\n  $organizationId: String!\n  $organizationExists: Boolean!\n  $organizationLocationsSortingValues: [LocationOrderInput!]!\n  $organizationTeamsSortingValues: [TeamOrderInput!]!\n  $locationNameSearchText: String\n  $teamNameSearchText: String\n) {\n  organization(id: $organizationId) {\n    id\n    name\n    logoUrl\n    canModify\n    canViewAnalytics\n  }\n  ...organizationLocationsTab_query\n  ...organizationTeamsTab_query\n  ...organizationBillingTab_query\n  ...organizationOfferingTab_query\n}\n\nfragment organizationAvailableOfferings_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    hasAttachedPaymentMethod\n    availableOfferings {\n      code\n      name\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationBillingInfo_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n  }\n  organizationBillingInfo(organizationId: $organizationId) {\n    organizationId\n    email\n    addressLine1\n    addressLine2\n    suburb\n    city\n    province\n    zipcode\n    country\n  }\n}\n\nfragment organizationBillingTab_query on Query {\n  ...organizationPaymentMethods_query\n  ...organizationBillingInfo_query\n}\n\nfragment organizationLocationsTab_query on Query {\n  locations(first: 50, where: {organizationId: $organizationId, nameContains: $locationNameSearchText}, orderBy: $organizationLocationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueId\n          name\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  organization(id: $organizationId) {\n    id\n    canModify\n  }\n}\n\nfragment organizationOfferingTab_query on Query {\n  ...organizationOffering_query\n  ...organizationAvailableOfferings_query\n  organization(id: $organizationId) {\n    id\n    availableOfferings {\n      code\n    }\n  }\n}\n\nfragment organizationOffering_query on Query {\n  organization(id: $organizationId) {\n    id\n    name\n    offering {\n      id\n      name\n      start\n      end\n      unitPrice\n      featureSet {\n        name\n        description\n      }\n    }\n  }\n}\n\nfragment organizationPaymentMethods_query on Query {\n  organization(id: $organizationId) {\n    id\n  }\n  organizationPaymentMethodsDetails(organizationId: $organizationId) @include(if: $organizationExists) {\n    id\n    cardBrand\n    cardExpiryMonth\n    cardExpiryYear\n    cardLastFourDigit\n  }\n}\n\nfragment organizationTeamsTab_query on Query {\n  teams(first: 50, where: {organizationId: $organizationId, nameContains: $teamNameSearchText}, orderBy: $organizationTeamsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        organization {\n          uniqueId\n          name\n        }\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  organization(id: $organizationId) {\n    id\n    canModify\n  }\n}\n"
  }
};
})();

(node as any).hash = "228d3d14b428d08532b989c72c90b979";

export default node;
