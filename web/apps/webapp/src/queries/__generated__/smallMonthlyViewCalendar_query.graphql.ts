/**
 * @generated SignedSource<<2a5de12539822836a3b34f73b8eee5d0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type smallMonthlyViewCalendar_query$data = {
  readonly me: {
    readonly defaultOrganization: {
      readonly uniqueId: string;
    } | null | undefined;
    readonly familyName: string | null | undefined;
    readonly givenName: string | null | undefined;
    readonly id: string;
    readonly middleName: string | null | undefined;
    readonly name: string | null | undefined;
    readonly photoUrl: string | null | undefined;
  } | null | undefined;
  readonly monthlyBookings: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly customer: {
          readonly photoUrl: string | null | undefined;
        };
        readonly from: any;
        readonly id: string;
        readonly notes: string | null | undefined;
        readonly to: any;
        readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
      };
    }>;
  } | null | undefined;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingCard_query">;
  readonly " $fragmentType": "smallMonthlyViewCalendar_query";
};
export type smallMonthlyViewCalendar_query$key = {
  readonly " $data"?: smallMonthlyViewCalendar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"smallMonthlyViewCalendar_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "monthlyBookings"
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
  "name": "photoUrl",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "bookingDetailsSelectorOrganizationMembersSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "bookingPeopleNameSearchText"
    },
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
      "name": "dateToGetAvailableDesks"
    },
    {
      "kind": "RootArgument",
      "name": "deskIdsToIncludeToGetAvailableDesks"
    },
    {
      "kind": "RootArgument",
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
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
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
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
      "operation": require('./smallMonthlyViewCalendarPaginationQuery.graphql')
    }
  },
  "name": "smallMonthlyViewCalendar_query",
  "selections": [
    {
      "alias": "monthlyBookings",
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
      "name": "__SmallMonthlyViewCalendar_monthlyBookings_connection",
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
                (v1/*: any*/),
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
                    (v2/*: any*/)
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
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v1/*: any*/),
        (v3/*: any*/),
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
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "CustomerOrganizationDetails",
          "kind": "LinkedField",
          "name": "defaultOrganization",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": [
        (v1/*: any*/),
        (v3/*: any*/)
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingCard_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "3d0a6ed27da2b50e823e435f42a72a97";

export default node;
