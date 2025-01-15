/**
 * @generated SignedSource<<03cb0a6cc7c0ed34712c0bff3ba2c5e1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingsWeekGrid_allBookings_refetchableFragment$variables = {
  from?: any | null | undefined;
  locationId: string;
  organizationId: string;
  teamId: string;
  to?: any | null | undefined;
};
export type bookingsWeekGrid_allBookings_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"bookingsWeekGrid_allBookings_query">;
};
export type bookingsWeekGrid_allBookings_refetchableFragment = {
  response: bookingsWeekGrid_allBookings_refetchableFragment$data;
  variables: bookingsWeekGrid_allBookings_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "from"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "teamId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "to"
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
  (v2/*: any*/)
],
v4 = [
  (v1/*: any*/),
  (v2/*: any*/),
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
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "bookingsWeekGrid_allBookings_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "bookingsWeekGrid_allBookings_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "bookingsWeekGrid_allBookings_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": [
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
                "variableName": "from"
              },
              {
                "items": [
                  {
                    "kind": "Variable",
                    "name": "locationIds.0",
                    "variableName": "locationId"
                  }
                ],
                "kind": "ListValue",
                "name": "locationIds"
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
                "items": [
                  {
                    "kind": "Variable",
                    "name": "teamIds.0",
                    "variableName": "teamId"
                  }
                ],
                "kind": "ListValue",
                "name": "teamIds"
              },
              {
                "kind": "Variable",
                "name": "toLT",
                "variableName": "to"
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
                    "name": "type",
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
                      (v2/*: any*/),
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingOrganizationCustomTagDetails",
                        "kind": "LinkedField",
                        "name": "customTags",
                        "plural": true,
                        "selections": (v4/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "BookingOrganizationZoneDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v4/*: any*/),
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
    "cacheID": "18bf2a9afe1a1f8bb57015b8faec7a1c",
    "id": null,
    "metadata": {},
    "name": "bookingsWeekGrid_allBookings_refetchableFragment",
    "operationKind": "query",
    "text": "query bookingsWeekGrid_allBookings_refetchableFragment(\n  $from: DateTime\n  $locationId: String!\n  $organizationId: String!\n  $teamId: String!\n  $to: DateTime\n) {\n  ...bookingsWeekGrid_allBookings_query\n}\n\nfragment bookingsWeekGrid_allBookings_query on Query {\n  bookings(where: {organizationIds: [$organizationId], locationIds: [$locationId], teamIds: [$teamId], fromGTE: $from, toLT: $to, combineOrganizationsLocationsTeams: true}) {\n    totalCount\n    edges {\n      node {\n        id\n        from\n        to\n        type\n        customer {\n          uniqueId\n          name\n          givenName\n          middleName\n          familyName\n          photoUrl\n        }\n        location {\n          name\n        }\n        team {\n          name\n        }\n        desks {\n          name\n          customTags {\n            uniqueId\n            name\n            color\n          }\n          zones {\n            uniqueId\n            name\n            color\n          }\n        }\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6a4a92e3f14fb3c7a4398cce8ba060a9";

export default node;
