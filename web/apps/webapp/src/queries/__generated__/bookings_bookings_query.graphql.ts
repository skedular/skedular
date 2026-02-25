/**
 * @generated SignedSource<<f7583e6eabd80a95049e1d02574fe9cc>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type BookingCategory = "ANNUAL_LEAVE" | "CLIENT_OFFICE" | "NON_WORKING_DAY" | "SICK_LEAVE" | "TRAVELING_FOR_WORK" | "VACATION" | "WELLBEING_LEAVE" | "WORKING_FROM_COWORKING_SPACE" | "WORKING_FROM_HOME" | "WORKING_FROM_OFFICE" | "%future added value";
export type BookingChannel = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type bookings_bookings_query$data = {
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
        readonly category: {
          readonly category: BookingCategory;
          readonly name: string;
        };
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
          readonly id: string;
          readonly name: string;
        }>;
        readonly involvedOrganizations: ReadonlyArray<{
          readonly id: string;
        }>;
        readonly involvedTeams: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
        }>;
        readonly notes: string | null | undefined;
        readonly until: any;
        readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "bookings_bookings_query";
};
export type bookings_bookings_query$key = {
  readonly " $data"?: bookings_bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookings_bookings_query">;
};

import bookings_bookings_refetchableFragment_graphql from './bookings_bookings_refetchableFragment.graphql';

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
v3 = [
  (v1/*: any*/),
  (v2/*: any*/)
],
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "color",
  "storageKey": null
},
v5 = [
  (v1/*: any*/),
  (v2/*: any*/),
  (v4/*: any*/)
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
      "name": "customerIds"
    },
    {
      "kind": "RootArgument",
      "name": "locationIds"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
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
      "operation": bookings_bookings_refetchableFragment_graphql
    }
  },
  "name": "bookings_bookings_query",
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
              "name": "customerIds",
              "variableName": "customerIds"
            },
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
              "items": [
                {
                  "kind": "Variable",
                  "name": "organizationUniqueAlphanumericNames.0",
                  "variableName": "organizationUniqueAlphanumericName"
                }
              ],
              "kind": "ListValue",
              "name": "organizationUniqueAlphanumericNames"
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
      "name": "__bookings_bookings_connection",
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
                  "concreteType": "BookingCategoryDetails",
                  "kind": "LinkedField",
                  "name": "category",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "category",
                      "storageKey": null
                    },
                    (v2/*: any*/)
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
                  "concreteType": "OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "involvedOrganizations",
                  "plural": true,
                  "selections": [
                    (v1/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationDetails",
                  "kind": "LinkedField",
                  "name": "involvedLocations",
                  "plural": true,
                  "selections": (v3/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "TeamDetails",
                  "kind": "LinkedField",
                  "name": "involvedTeams",
                  "plural": true,
                  "selections": (v3/*: any*/),
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
                        (v1/*: any*/),
                        (v2/*: any*/),
                        (v4/*: any*/),
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OrganizationTagDetails",
                          "kind": "LinkedField",
                          "name": "customTags",
                          "plural": true,
                          "selections": (v5/*: any*/),
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "concreteType": "OrganizationTagDetails",
                          "kind": "LinkedField",
                          "name": "zones",
                          "plural": true,
                          "selections": (v5/*: any*/),
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

(node as any).hash = "9c2ea6b930802f0afd1f192e3a41ed37";

export default node;
