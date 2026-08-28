/**
 * @generated SignedSource<<4002e940607fe4b1ecf5f08e06fbb575>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type PaymentStatus = "CONFIRMED" | "EXPIRED" | "NOT_SET" | "NO_PAYMENT_REQUIRED" | "PENDING" | "RECORD_NEVER_CREATED" | "REJECTED" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type customerBookingsHub_upcomingBookings_query$data = {
  readonly upcomingBookings: {
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly name: string;
          };
        }>;
        readonly from: any;
        readonly id: string;
        readonly involvedLocations: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly customDomain: string | null | undefined;
          readonly name: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly name: string;
        }>;
        readonly marketplaceBooking: {
          readonly paymentStatus: {
            readonly name: string;
            readonly type: PaymentStatus;
          };
        } | null | undefined;
        readonly recurringBooking: {
          readonly frequency: {
            readonly name: string;
          };
        } | null | undefined;
        readonly until: any;
      };
    }>;
    readonly pageInfo: {
      readonly endCursor: string | null | undefined;
      readonly hasNextPage: boolean;
    };
    readonly totalCount: number;
  };
  readonly " $fragmentType": "customerBookingsHub_upcomingBookings_query";
};
export type customerBookingsHub_upcomingBookings_query$key = {
  readonly " $data"?: customerBookingsHub_upcomingBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"customerBookingsHub_upcomingBookings_query">;
};

import customerBookingsHub_upcomingBookingsPaginationQuery_graphql from './customerBookingsHub_upcomingBookingsPaginationQuery.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "upcomingBookings"
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v1/*:: as any*/)
];
return {
  "argumentDefinitions": [
    {
      "defaultValue": 25,
      "kind": "LocalArgument",
      "name": "count"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "cursor"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "organizationCustomDomain"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "today"
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
      "operation": customerBookingsHub_upcomingBookingsPaginationQuery_graphql
    }
  },
  "name": "customerBookingsHub_upcomingBookings_query",
  "selections": [
    {
      "alias": "upcomingBookings",
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
              "variableName": "today"
            },
            {
              "kind": "Literal",
              "name": "includeMineOnly",
              "value": true
            },
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
      "concreteType": "ConnectionOfBookingEdge",
      "kind": "LinkedField",
      "name": "__customerBookingsHub_upcomingBookings_connection",
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
          "concreteType": "PageInfo",
          "kind": "LinkedField",
          "name": "pageInfo",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "hasNextPage",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "endCursor",
              "storageKey": null
            }
          ],
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
                  "name": "until",
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "involvedOrganizations",
                  "plural": true,
                  "selections": [
                    (v1/*:: as any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "customDomain",
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
                  "selections": (v2/*:: as any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "TeamDetails",
                  "kind": "LinkedField",
                  "name": "involvedTeams",
                  "plural": true,
                  "selections": (v2/*:: as any*/),
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
                      "selections": (v2/*:: as any*/),
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "MarketplaceBookingDetails",
                  "kind": "LinkedField",
                  "name": "marketplaceBooking",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "PaymentStatusDetails",
                      "kind": "LinkedField",
                      "name": "paymentStatus",
                      "plural": false,
                      "selections": [
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "type",
                          "storageKey": null
                        },
                        (v1/*:: as any*/)
                      ],
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "RecurringBookingDetails",
                  "kind": "LinkedField",
                  "name": "recurringBooking",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "BookingFrequencyDetails",
                      "kind": "LinkedField",
                      "name": "frequency",
                      "plural": false,
                      "selections": (v2/*:: as any*/),
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
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "b2f8514ecae45b9df6a346cdcc1eda53";

export default node;
