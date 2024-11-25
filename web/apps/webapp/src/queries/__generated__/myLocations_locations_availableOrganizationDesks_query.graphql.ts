/**
 * @generated SignedSource<<d14fe13022580a65d80a9a1477cdde46>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type myLocations_locations_availableOrganizationDesks_query$data = {
  readonly availableOrganizationDesks: ReadonlyArray<{
    readonly location: {
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly locations: {
    readonly __id: string;
    readonly edges: ReadonlyArray<{
      readonly node: {
        readonly desks: ReadonlyArray<{
          readonly id: string;
        }>;
        readonly id: string;
        readonly locationTags: ReadonlyArray<{
          readonly id: string;
          readonly name: string;
          readonly tagType: string;
        }>;
        readonly name: string;
        readonly physicalAddress: {
          readonly formattedAddress: string | null | undefined;
        } | null | undefined;
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
var v0 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
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
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./myLocations_locations_availableOrganizationDesks_refetchableFragment.graphql')
    }
  },
  "name": "myLocations_locations_availableOrganizationDesks_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "locationsSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "deskTypeIds",
              "variableName": "deskTypeIds"
            },
            (v0/*: any*/),
            {
              "kind": "Variable",
              "name": "zoneIds",
              "variableName": "zoneIds"
            }
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "LocationConnection",
      "kind": "LinkedField",
      "name": "locations",
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
                (v1/*: any*/),
                (v2/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "DeskDetails",
                  "kind": "LinkedField",
                  "name": "desks",
                  "plural": true,
                  "selections": [
                    (v1/*: any*/)
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationTagDetails",
                  "kind": "LinkedField",
                  "name": "locationTags",
                  "plural": true,
                  "selections": [
                    (v1/*: any*/),
                    (v2/*: any*/),
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "tagType",
                      "storageKey": null
                    }
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
    },
    {
      "alias": null,
      "args": [
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
        (v0/*: any*/)
      ],
      "concreteType": "BookingDeskDetails",
      "kind": "LinkedField",
      "name": "availableOrganizationDesks",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingLocationDetails",
          "kind": "LinkedField",
          "name": "location",
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "8b62b81da8ca4732ccf0de4318fe922d";

export default node;
