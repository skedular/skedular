/**
 * @generated SignedSource<<9ebdb0eef4bcb8d671980d5edec063bb>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myLocations_locations_availableOrganizationDesks_query$data = {
  readonly availableDesks: ReadonlyArray<{
    readonly location: {
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly canDelete: boolean;
        readonly canModify: boolean;
        readonly deskTypes: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly desks: ReadonlyArray<{
          readonly id: string;
        }>;
        readonly hasFutureBooking: boolean;
        readonly id: string;
        readonly name: string;
        readonly organization: {
          readonly uniqueId: string;
        } | null | undefined;
        readonly physicalAddress: {
          readonly formattedAddress: string | null | undefined;
        } | null | undefined;
        readonly zones: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly " $fragmentSpreads": FragmentRefs<"myLocationCard_LocationDetails">;
      };
    }>;
    readonly totalCount: number | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "myLocations_locations_availableOrganizationDesks_query";
};
export type myLocations_locations_availableOrganizationDesks_query$key = {
  readonly " $data"?: myLocations_locations_availableOrganizationDesks_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"myLocations_locations_availableOrganizationDesks_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "locations"
],
v1 = {
  "kind": "Variable",
  "name": "deskTypeIds",
  "variableName": "deskTypeIds"
},
v2 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
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
  (v5/*: any*/)
],
v8 = [
  (v6/*: any*/)
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
      "name": "deskTypeIds"
    },
    {
      "kind": "RootArgument",
      "name": "locationsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "todayDate"
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
      "operation": require('./myLocations_locations_availableOrganizationDesks_refetchableFragment.graphql')
    }
  },
  "name": "myLocations_locations_availableOrganizationDesks_query",
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
      "concreteType": "LocationConnection",
      "kind": "LinkedField",
      "name": "__myLocations_locations_connection",
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
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "deskTypes",
                  "plural": true,
                  "selections": (v7/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v7/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "DeskDetails",
                  "kind": "LinkedField",
                  "name": "desks",
                  "plural": true,
                  "selections": [
                    (v4/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationAddressDetails",
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
                  "concreteType": "LocationOrganizationDetails",
                  "kind": "LinkedField",
                  "name": "organization",
                  "plural": false,
                  "selections": (v8/*: any*/),
                  "storageKey": null
                },
                {
                  "args": null,
                  "kind": "FragmentSpread",
                  "name": "myLocationCard_LocationDetails"
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
            {
              "kind": "Literal",
              "name": "combineDeskTypesZones",
              "value": true
            },
            {
              "kind": "Variable",
              "name": "date",
              "variableName": "todayDate"
            },
            {
              "kind": "Literal",
              "name": "deskIdsToInclude",
              "value": []
            },
            (v1/*: any*/),
            (v2/*: any*/),
            (v3/*: any*/)
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "BookingDeskDetails",
      "kind": "LinkedField",
      "name": "availableDesks",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingLocationDetails",
          "kind": "LinkedField",
          "name": "location",
          "plural": false,
          "selections": (v8/*: any*/),
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

(node as any).hash = "b330461725b896ff86d8b99f899bd7f5";

export default node;
