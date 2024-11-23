/**
 * @generated SignedSource<<0a327a2efabdbebe4a08b724b81a2aab>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBookings_bookings_refetchableFragment$variables = {
  bookingsSearchCriteriaFrom?: any | null | undefined;
  bookingsSearchCriteriaTo?: any | null | undefined;
  locationIds?: ReadonlyArray<string> | null | undefined;
  organizationId: string;
  teamIds?: ReadonlyArray<string> | null | undefined;
};
export type myBookings_bookings_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myBookings_bookings_query">;
};
export type myBookings_bookings_refetchableFragment = {
  response: myBookings_bookings_refetchableFragment$data;
  variables: myBookings_bookings_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingsSearchCriteriaFrom"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "bookingsSearchCriteriaTo"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamIds"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
  (v1/*: any*/),
  (v2/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myBookings_bookings_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myBookings_bookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myBookings_bookings_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
          {
            "kind": "Literal",
            "name": "orderBy",
            "value": [
              {
                "direction": "Ascending",
                "field": "From"
              }
            ]
          },
          {
            "fields": [
              {
                "kind": "Literal",
                "name": "combineOrganizationsLocationsTeams",
                "value": true
              },
              {
                "kind": "Variable",
                "name": "fromGTE",
                "variableName": "bookingsSearchCriteriaFrom"
              },
              {
                "kind": "Variable",
                "name": "fromLTE",
                "variableName": "bookingsSearchCriteriaTo"
              },
              {
                "kind": "Literal",
                "name": "includeFutureBookingsOnly",
                "value": true
              },
              {
                "kind": "Variable",
                "name": "locationIds",
                "variableName": "locationIds"
              },
              {
                "items": [
                  {
                    "kind": "Variable",
                    "name": "organizationIds.0",
                    "variableName": "organizationId"
                  }
                ],
                "kind": "ListValue",
                "name": "organizationIds"
              },
              {
                "kind": "Variable",
                "name": "teamIds",
                "variableName": "teamIds"
              }
            ],
            "kind": "ObjectValue",
            "name": "where"
          }
        ],
        "concreteType": "BookingConnection",
        "kind": "LinkedField",
        "name": "bookings",
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
            "concreteType": "BookingEdge",
            "kind": "LinkedField",
            "name": "edges",
            "plural": true,
            "selections": [
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingDetails",
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
                    "name": "from",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "to",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "kind": "ScalarField",
                    "name": "notes",
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingCustomerDetails",
                    "kind": "LinkedField",
                    "name": "customer",
                    "plural": false,
                    "selections": [
                      (v1/*: any*/),
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
                    "concreteType": "BookingLocationDetails",
                    "kind": "LinkedField",
                    "name": "location",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingTeamDetails",
                    "kind": "LinkedField",
                    "name": "team",
                    "plural": false,
                    "selections": (v3/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "BookingDeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": [
                      (v1/*: any*/),
                      (v2/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingLocationTagDetails",
                        "kind": "LinkedField",
                        "name": "locationTags",
                        "plural": true,
                        "selections": [
                          (v1/*: any*/),
                          (v2/*: any*/),
                          {
                            "alias": null,
                            "args": null,
                            "kind": "ScalarField",
                            "name": "tagType",
                            "storageKey": null
                          }
                        ],
                        "storageKey": null
                      }
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
      }
    ]
  },
  "params": {
    "cacheID": "1c5460c1288794abefa27961152828b9",
    "id": null,
    "metadata": {},
    "name": "myBookings_bookings_refetchableFragment",
    "operationKind": "query",
    "text": "query myBookings_bookings_refetchableFragment(\n  $bookingsSearchCriteriaFrom: DateTime\n  $bookingsSearchCriteriaTo: DateTime\n  $locationIds: [String!]\n  $organizationId: String!\n  $teamIds: [String!]\n) {\n  ...myBookings_bookings_query\n}\n\nfragment myBookings_bookings_query on Query {\n  bookings(where: {organizationIds: [$organizationId], locationIds: $locationIds, teamIds: $teamIds, fromGTE: $bookingsSearchCriteriaFrom, fromLTE: $bookingsSearchCriteriaTo, includeFutureBookingsOnly: true, combineOrganizationsLocationsTeams: true}, orderBy: [{field: From, direction: Ascending}]) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        notes\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        location {\n          uniqueId\n          name\n        }\n        team {\n          uniqueId\n          name\n        }\n        desks {\n          uniqueId\n          name\n          locationTags {\n            uniqueId\n            name\n            tagType\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "31569185e78d2675b2dabaac413b282f";

export default node;
