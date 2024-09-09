/**
 * @generated SignedSource<<8561f31e27413bb07a86d84846fd3f50>>
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
export type LocationOrderInput = {
  direction: OrderDirection;
  field?: LocationOrderField | null | undefined;
};
export type pageLocations_rootQuery$variables = {
  locationNameSearchText: string;
  locationsSortingValues: ReadonlyArray<LocationOrderInput>;
};
export type pageLocations_rootQuery$data = {
  readonly locationCustomerRecordSynced: boolean;
  readonly " $fragmentSpreads": FragmentRefs<"locations_query" | "rootShell_query">;
};
export type pageLocations_rootQuery = {
  response: pageLocations_rootQuery$data;
  variables: pageLocations_rootQuery$variables;
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
  "name": "locationsSortingValues"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "locationCustomerRecordSynced",
  "storageKey": null
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
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v6 = [
  {
    "kind": "Literal",
    "name": "first",
    "value": 50
  },
  {
    "kind": "Variable",
    "name": "orderBy",
    "variableName": "locationsSortingValues"
  },
  {
    "fields": [
      {
        "kind": "Variable",
        "name": "nameContains",
        "variableName": "locationNameSearchText"
      }
    ],
    "kind": "ObjectValue",
    "name": "where"
  }
];
return {
  "fragment": {
    "argumentDefinitions": [
      (v0/*: any*/),
      (v1/*: any*/)
    ],
    "kind": "Fragment",
    "metadata": null,
    "name": "pageLocations_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "rootShell_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [
      (v1/*: any*/),
      (v0/*: any*/)
    ],
    "kind": "Operation",
    "name": "pageLocations_rootQuery",
    "selections": [
      (v2/*: any*/),
      {
        "alias": null,
        "args": null,
        "concreteType": "CustomerDetails",
        "kind": "LinkedField",
        "name": "me",
        "plural": false,
        "selections": [
          (v3/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerEmail",
            "kind": "LinkedField",
            "name": "email",
            "plural": false,
            "selections": [
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "email",
                "storageKey": null
              },
              (v3/*: any*/),
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "verified",
                "storageKey": null
              }
            ],
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "title",
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
          (v4/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "CustomerLocationDetails",
            "kind": "LinkedField",
            "name": "defaultLocations",
            "plural": true,
            "selections": [
              (v5/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": (v6/*: any*/),
        "concreteType": "LocationConnection",
        "kind": "LinkedField",
        "name": "locations",
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
                  (v3/*: any*/),
                  (v4/*: any*/),
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
                    "concreteType": "LocationOrganizationDetails",
                    "kind": "LinkedField",
                    "name": "organization",
                    "plural": false,
                    "selections": [
                      (v5/*: any*/),
                      (v4/*: any*/)
                    ],
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "deskCapacity",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "hasFutureBooking",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "canModify",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "canDelete",
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
        "args": (v6/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "locations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      }
    ]
  },
  "params": {
    "cacheID": "63ab232d652bcaec68977cacc62dc98d",
    "id": null,
    "metadata": {},
    "name": "pageLocations_rootQuery",
    "operationKind": "query",
    "text": "query pageLocations_rootQuery(\n  $locationsSortingValues: [LocationOrderInput!]!\n  $locationNameSearchText: String!\n) {\n  locationCustomerRecordSynced\n  ...rootShell_query\n  ...locations_query\n}\n\nfragment locationCard_LocationDetails on LocationDetails {\n  id\n  name\n  about\n  organization {\n    uniqueId\n    name\n  }\n  deskCapacity\n  hasFutureBooking\n  canModify\n  canDelete\n}\n\nfragment locationCard_Query on Query {\n  me {\n    id\n    defaultLocations {\n      uniqueId\n    }\n  }\n}\n\nfragment locations_query on Query {\n  locations(first: 50, where: {nameContains: $locationNameSearchText}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        ...locationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  ...locationCard_Query\n}\n\nfragment logrocket_query on Query {\n  me {\n    id\n    email {\n      email\n      id\n    }\n    title\n    givenName\n    middleName\n    familyName\n  }\n}\n\nfragment mainRootLayout_query on Query {\n  me {\n    email {\n      email\n      verified\n      id\n    }\n    givenName\n    middleName\n    familyName\n    photoUrl\n    id\n  }\n  ...newFeedbackDialog_query\n}\n\nfragment newFeedbackDialog_query on Query {\n  me {\n    name\n    givenName\n    middleName\n    familyName\n    id\n  }\n}\n\nfragment observability_query on Query {\n  ...logrocket_query\n}\n\nfragment rootShell_query on Query {\n  me {\n    id\n  }\n  ...observability_query\n  ...mainRootLayout_query\n}\n"
  }
};
})();

(node as any).hash = "45896658b0c6947b3eceba05a3eea4f2";

export default node;
