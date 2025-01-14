/**
 * @generated SignedSource<<8fe2ec7f4dc70935451836be2caed0d4>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type smallMonthlyViewCalendar_bookings_query$data = {
  readonly bookings: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly customer: {
          readonly familyName: string | null | undefined;
          readonly givenName: string | null | undefined;
          readonly middleName: string | null | undefined;
          readonly name: string | null | undefined;
          readonly photoUrl: string | null | undefined;
          readonly uniqueId: string;
        };
        readonly desks: ReadonlyArray<{
          readonly customTags: ReadonlyArray<{
            readonly name: string;
            readonly uniqueId: string;
          }>;
          readonly name: string;
          readonly uniqueId: string;
          readonly zones: ReadonlyArray<{
            readonly name: string;
            readonly uniqueId: string;
          }>;
        }>;
        readonly from: any;
        readonly id: string;
        readonly location: {
          readonly name: string;
          readonly uniqueId: string;
        } | null | undefined;
        readonly notes: string | null | undefined;
        readonly organization: {
          readonly name: string;
          readonly uniqueId: string;
        } | null | undefined;
        readonly team: {
          readonly name: string;
          readonly uniqueId: string;
        } | null | undefined;
        readonly to: any;
        readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
      };
    }>;
  } | null | undefined;
  readonly " $fragmentType": "smallMonthlyViewCalendar_bookings_query";
};
export type smallMonthlyViewCalendar_bookings_query$key = {
  readonly " $data"?: smallMonthlyViewCalendar_bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"smallMonthlyViewCalendar_bookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "bookings"
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
  "argumentDefinitions": [
    {
      "defaultValue": 1000,
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
      "name": "monthlyCalendarDateFrom"
    },
    {
      "kind": "RootArgument",
      "name": "monthlyCalendarDateTo"
    },
    {
      "kind": "RootArgument",
      "name": "smallMonthlyViewCalendarBookingsSortingValues"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "connection": [
      {
        "count": "count",
        "cursor": "cursor",
        "direction": "forward",
        "path": (v0/*: any*/)
      }
    ],
    "refetch": {
      "connection": {
        "forward": {
          "count": "count",
          "cursor": "cursor"
        },
        "backward": null,
        "path": (v0/*: any*/)
      },
      "fragmentPathInResult": [],
      "operation": require('./smallMonthlyViewCalendarPaginationQuery_bookings_refetchableFragment.graphql')
    }
  },
  "name": "smallMonthlyViewCalendar_bookings_query",
  "selections": [
    {
      "alias": "bookings",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "smallMonthlyViewCalendarBookingsSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "fromGTE",
              "variableName": "monthlyCalendarDateFrom"
            },
            {
              "kind": "Literal",
              "name": "includeMineOnly",
              "value": true
            },
            {
              "kind": "Variable",
              "name": "toLT",
              "variableName": "monthlyCalendarDateTo"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "BookingConnection",
      "kind": "LinkedField",
      "name": "__SmallMonthlyViewCalendar_bookings_connection",
      "plural": false,
      "selections": [
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
                  "concreteType": "BookingOrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": (v3/*: any*/),
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
                      "concreteType": "BookingOrganizationCustomTagDetails",
                      "kind": "LinkedField",
                      "name": "customTags",
                      "plural": true,
                      "selections": (v3/*: any*/),
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "BookingOrganizationZoneDetails",
                      "kind": "LinkedField",
                      "name": "zones",
                      "plural": true,
                      "selections": (v3/*: any*/),
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "bookingCard_BookingDetails"
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

(node as any).hash = "50f8820e965d3a0101808d378acaabd8";

export default node;
