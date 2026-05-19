/**
 * @generated SignedSource<<b0b0ef679034c44ce037b94f8aa15f16>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type myBookings_bookings_query$data = {
  readonly bookings: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly color: string | null | undefined;
            readonly customTags: ReadonlyArray<{
              readonly color: string | null | undefined;
              readonly id: string;
              readonly name: string;
            }>;
            readonly id: string;
            readonly name: string;
            readonly zones: ReadonlyArray<{
              readonly color: string | null | undefined;
              readonly id: string;
              readonly name: string;
            }>;
          };
        }>;
        readonly channel: {
          readonly channel: BookingChannel;
        };
        readonly from: any;
        readonly id: string;
        readonly involvedCustomers: ReadonlyArray<{
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly id: string;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
        }>;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
          readonly uniqueId: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly notes: string | null | undefined;
        readonly until: any;
        readonly " $fragmentSpreads": FragmentRefs<"myBookingCard_BookingDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "myBookings_bookings_query";
};
export type myBookings_bookings_query$key = {
  readonly " $data"?: myBookings_bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookings_bookings_query">;
};

import myBookings_bookings_refetchableFragment_graphql from './myBookings_bookings_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "bookings"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v4 = [
  (v1/*:: as any*/),
  (v2/*:: as any*/),
  (v3/*:: as any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingsSearchCriteriaFrom"
    },
    {
      "kind": "RootArgument",
      "name": "bookingsSearchCriteriaTo"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "count"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "cursor"
    },
    {
      "kind": "RootArgument",
      "name": "locationIds"
    },
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    },
    {
      "kind": "RootArgument",
      "name": "teamIds"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*:: as any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*:: as any*/)
      },
      "fragmentPathInResult": [],
      "operation": myBookings_bookings_refetchableFragment_graphql
    }
  },
  "name": "myBookings_bookings_query",
  "selections": [
    {
      "alias": "bookings",
      "args": [
        {
          "kind": "Literal",
          "name": "orderBy",
          "value": [
            {
              "direction": "ASCENDING",
              "field": "FROM"
            }
          ]
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "fromGte",
              "variableName": "bookingsSearchCriteriaFrom"
            },
            {
              "kind": "Variable",
              "name": "fromLte",
              "variableName": "bookingsSearchCriteriaTo"
            },
            {
              "kind": "Variable",
              "name": "locationIds",
              "variableName": "locationIds"
            },
            {
              "kind": "Variable",
              "name": "organizationCustomDomain",
              "variableName": "organizationCustomDomain"
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
      "concreteType": "ConnectionOfBookingEdge",
      "kind": "LinkedField",
      "name": "__myBookings_bookings_connection",
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
                (v1/*:: as any*/),
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
                  "name": "until",
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
                  "concreteType": "BookingChannelDetails",
                  "kind": "LinkedField",
                  "name": "channel",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "channel",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "CustomerDetails",
                  "kind": "LinkedField",
                  "name": "involvedCustomers",
                  "plural": true,
                  "selections": [
                    (v1/*:: as any*/),
                    (v2/*:: as any*/),
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
                  "concreteType": "Booking_LocationDetails",
                  "kind": "LinkedField",
                  "name": "involvedLocations",
                  "plural": true,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "uniqueId",
                      "storageKey": null
                    },
                    (v2/*:: as any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "TeamDetails",
                  "kind": "LinkedField",
                  "name": "involvedTeams",
                  "plural": true,
                  "selections": [
                    (v1/*:: as any*/),
                    (v2/*:: as any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "BookingResourceDetails",
                  "kind": "LinkedField",
                  "name": "bookingResources",
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
                        (v1/*:: as any*/),
                        (v2/*:: as any*/),
                        (v3/*:: as any*/),
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OrganizationTagDetails",
                          "kind": "LinkedField",
                          "name": "customTags",
                          "plural": true,
                          "selections": (v4/*:: as any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OrganizationTagDetails",
                          "kind": "LinkedField",
                          "name": "zones",
                          "plural": true,
                          "selections": (v4/*:: as any*/),
                          "storageKey": null
                        }
                      ],
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "myBookingCard_BookingDetails"
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "0474e466a532512469601eae43239fcf";

export default node;
