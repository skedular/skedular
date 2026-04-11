/**
 * @generated SignedSource<<e889d37d539881774f9f7cf607fc83b8>>
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
        readonly extraMetadata: {
          readonly contactDetails: {
            readonly contactEmails: ReadonlyArray<string> | null | undefined;
            readonly contactPhones: ReadonlyArray<string> | null | undefined;
          } | null | undefined;
        } | null | undefined;
        readonly id: string;
        readonly physicalAddress: {
          readonly latitude: number | null | undefined;
          readonly longitude: number | null | undefined;
        } | null | undefined;
        readonly resources: {
          readonly totalCount: number;
        };
        readonly " $fragmentSpreads": FragmentRefs<"locationCard_LocationDetails">;
      };
    }>;
    readonly totalCount: number;
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
  "name": "organizationCustomDomain",
  "variableName": "organizationCustomDomain"
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
  "name": "totalCount",
  "storageKey": null
};
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
      "name": "locationNotContactedYet"
    },
    {
      "kind": "RootArgument",
      "name": "locationsSortingValues"
    },
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
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
            {
              "kind": "Variable",
              "name": "notContactedYet",
              "variableName": "locationNotContactedYet"
            },
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
        (v4/*: any*/),
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
                  "concreteType": "ConnectionOfResourceEdge",
                  "kind": "LinkedField",
                  "name": "resources",
                  "plural": false,
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
                      "name": "longitude",
                      "storageKey": null
                    },
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "latitude",
                      "storageKey": null
                    }
                  ],
                  "storageKey": null
                },
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "LocationExtraMetadata",
                  "kind": "LinkedField",
                  "name": "extraMetadata",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "concreteType": "ContactDetails",
                      "kind": "LinkedField",
                      "name": "contactDetails",
                      "plural": false,
                      "selections": [
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "contactEmails",
                          "storageKey": null
                        },
                        {
                          "alias": null,
                          "args": null,
                          "kind": "ScalarField",
                          "name": "contactPhones",
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

(node as any).hash = "8cdd20424351ef080a19ebc0c28ae9c0";

export default node;
