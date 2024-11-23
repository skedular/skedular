/**
 * @generated SignedSource<<ee3cbea360885e7456ffbf3bccdaee7b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myBookings_bookings_query$data = {
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
          readonly locationTags: ReadonlyArray<{
            readonly name: string;
            readonly tagType: string | null | undefined;
            readonly uniqueId: string;
          }>;
          readonly name: string;
          readonly uniqueId: string;
        }>;
        readonly from: any;
        readonly id: string;
        readonly location: {
          readonly name: string;
          readonly uniqueId: string;
        } | null | undefined;
        readonly notes: string | null | undefined;
        readonly team: {
          readonly name: string;
          readonly uniqueId: string;
        } | null | undefined;
        readonly to: any;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "myBookings_bookings_query";
};
export type myBookings_bookings_query$key = {
  readonly " $data"?: myBookings_bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myBookings_bookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
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
      "kind": "RootArgument",
      "name": "locationIds"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "teamIds"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./myBookings_bookings_refetchableFragment.graphql')
    }
  },
  "name": "myBookings_bookings_query",
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
                    (v0/*: any*/),
                    (v1/*: any*/),
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
                  "selections": (v2/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "BookingTeamDetails",
                  "kind": "LinkedField",
                  "name": "team",
                  "plural": false,
                  "selections": (v2/*: any*/),
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
                    (v0/*: any*/),
                    (v1/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "BookingLocationTagDetails",
                      "kind": "LinkedField",
                      "name": "locationTags",
                      "plural": true,
                      "selections": [
                        (v0/*: any*/),
                        (v1/*: any*/),
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
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "31569185e78d2675b2dabaac413b282f";

export default node;
