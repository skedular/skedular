/**
 * @generated SignedSource<<a13ed94265c9e8cfaac1866c52a254d7>>
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
        readonly deskTypes: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
        readonly desks: ReadonlyArray<{
          readonly id: string;
        }>;
        readonly id: string;
        readonly name: string;
        readonly physicalAddress: {
          readonly formattedAddress: string | null | undefined;
        } | null | undefined;
        readonly zones: ReadonlyArray<{
          readonly name: string | null | undefined;
          readonly uniqueId: string;
        }>;
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
  "name": "deskTypeIds",
  "variableName": "deskTypeIds"
},
v1 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v2 = {
  "kind": "Variable",
  "name": "zoneIds",
  "variableName": "zoneIds"
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v6 = [
  (v5/*: any*/),
  (v4/*: any*/)
];
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
            (v0/*: any*/),
            (v1/*: any*/),
            (v2/*: any*/)
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
                (v3/*: any*/),
                (v4/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "deskTypes",
                  "plural": true,
                  "selections": (v6/*: any*/),
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "Organization_OrganizationTagDetails",
                  "kind": "LinkedField",
                  "name": "zones",
                  "plural": true,
                  "selections": (v6/*: any*/),
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
                    (v3/*: any*/)
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
            (v0/*: any*/),
            (v1/*: any*/),
            (v2/*: any*/)
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
          "selections": [
            (v5/*: any*/)
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

(node as any).hash = "9025cee5ba325270ac15144a037a3f6c";

export default node;
