/**
 * @generated SignedSource<<3c80bacb12b84080dfa84a289e82e7cb>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type floorPlans_bookings_query$data = {
  readonly bookings: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly bookingResources: ReadonlyArray<{
          readonly resource: {
            readonly id: string;
          };
        }>;
        readonly category: {
          readonly name: string;
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
        readonly until: any;
        readonly " $fragmentSpreads": FragmentRefs<"bookingCard_BookingDetails">;
      };
    }>;
    readonly totalCount: number;
  };
  readonly " $fragmentType": "floorPlans_bookings_query";
};
export type floorPlans_bookings_query$key = {
  readonly " $data"?: floorPlans_bookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"floorPlans_bookings_query">;
};

import floorPlans_bookings_refetchableFragment_graphql from './floorPlans_bookings_refetchableFragment.graphql';

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
};
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
      "name": "locationId"
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
      "operation": floorPlans_bookings_refetchableFragment_graphql
    }
  },
  "name": "floorPlans_bookings_query",
  "selections": [
    {
      "alias": "bookings",
      "args": [
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
              "items": [
                {
                  "kind": "Variable",
                  "name": "locationIds.0",
                  "variableName": "locationId"
                }
              ],
              "kind": "ListValue",
              "name": "locationIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfBookingEdge",
      "kind": "LinkedField",
      "name": "__floorPlans_bookings_connection",
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
                  "concreteType": "BookingCategoryDetails",
                  "kind": "LinkedField",
                  "name": "category",
                  "plural": false,
                  "selections": [
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
                        (v1/*:: as any*/)
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

(node as any).hash = "92a46f8479609065267f07c144470eab";

export default node;
