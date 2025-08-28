/**
 * @generated SignedSource<<e76e81411996cdb5ce39eea047f7c9ad>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocations_locations_availableOrganizationResources_query$data = {
  readonly availableResources: ReadonlyArray<{
    readonly location: {
      readonly uniqueId: string;
    } | null | undefined;
  }>;
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly canDelete: boolean;
        readonly canModify: boolean;
        readonly customTags: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly hasFutureBooking: boolean;
        readonly id: string;
        readonly name: string;
        readonly organization: {
          readonly uniqueAlphanumericName: string | null | undefined;
        };
        readonly physicalAddress: {
          readonly formattedAddress: string | null | undefined;
        } | null | undefined;
        readonly resources: ReadonlyArray<{
          readonly id: string;
        }>;
        readonly zones: ReadonlyArray<{
          readonly color: string | null | undefined;
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly " $fragmentSpreads": FragmentRefs<"locationCard_LocationDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly " $fragmentType": "organizationLocations_locations_availableOrganizationResources_query";
};
export type organizationLocations_locations_availableOrganizationResources_query$key = {
  readonly " $data"?: organizationLocations_locations_availableOrganizationResources_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocations_locations_availableOrganizationResources_query">;
};

import organizationLocations_locations_availableOrganizationResources_refetchableFragment_graphql from './organizationLocations_locations_availableOrganizationResources_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = [
  "locations"
],
v1 = {
  "kind": "Variable",
  "name": "customTagIds",
  "variableName": "customTagIds"
},
v2 = {
  "kind": "Variable",
  "name": "organizationUniqueAlphanumericName",
  "variableName": "organizationUniqueAlphanumericName"
},
v3 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v7 = [
  (v6/*: any*/),
  (v5/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
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
      "name": "customTagIds"
    },
    {
      "kind": "RootArgument",
      "name": "fromTodayDate"
    },
    {
      "kind": "RootArgument",
      "name": "locationsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    },
    {
      "kind": "RootArgument",
      "name": "untilTodayDate"
    },
    {
      "kind": "RootArgument",
      "name": "zoneIds"
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
      "operation": organizationLocations_locations_availableOrganizationResources_refetchableFragment_graphql
    }
  },
  "name": "organizationLocations_locations_availableOrganizationResources_query",
  "selections": [
    {
      "alias": "locations",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "locationsSortingValues"
        },
        {
          "fields": [
            (v1/*: any*/),
            (v2/*: any*/),
            (v3/*: any*/)
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "ConnectionOfLocationEdge",
      "kind": "LinkedField",
      "name": "__organizationLocations_locations_connection",
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
                (v4/*: any*/),
                (v5/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "customTags",
                  "plural": true,
                  "selections": (v7/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Location_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v7/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourceDetails",
                  "kind": "LinkedField",
                  "name": "resources",
                  "plural": true,
                  "selections": [
                    (v4/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationPhysicalAddressDetails",
                  "kind": "LinkedField",
                  "name": "physicalAddress",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "formattedAddress",
                      "storageKey": null
                    }
                  ],
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
                  "concreteType": "Location_OrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "uniqueAlphanumericName",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "locationCard_LocationDetails"
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
      "args": [
        {
          "fields": [
            (v1/*: any*/),
            {
              "kind": "Variable",
              "name": "from",
              "variableName": "fromTodayDate"
            },
            (v2/*: any*/),
            {
              "kind": "Variable",
              "name": "until",
              "variableName": "untilTodayDate"
            },
            (v3/*: any*/)
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "BookingResourceDetails",
      "kind": "LinkedField",
      "name": "availableResources",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "Booking_LocationDetails",
          "kind": "LinkedField",
          "name": "location",
          "plural": false,
          "selections": [
            (v6/*: any*/)
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

(node as any).hash = "54427292b4b950aff0d4de74fb08a465";

export default node;
