/**
 * @generated SignedSource<<d68c7e06698e4887be1a9dc965f927fd>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment, RefetchableFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingDetailsSelector_query$data = {
  readonly availableLocationDesks: ReadonlyArray<{
    readonly locationTags: ReadonlyArray<{
      readonly name: string;
      readonly tagType: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
  }>;
  readonly bookingDetailsSelectorQueryPaginatedOrganizationMembers: {
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
        readonly id: string;
      };
    }>;
    readonly totalCount: number | null | undefined;
  };
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly " $fragmentType": "bookingDetailsSelector_query";
};
export type bookingDetailsSelector_query$key = {
  readonly " $data"?: bookingDetailsSelector_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  "bookingDetailsSelectorQueryPaginatedOrganizationMembers"
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
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
},
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
      "defaultValue": 20,
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
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
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
      "operation": require('./bookingDetailsSelectorQuery.graphql')
    }
  },
  "name": "bookingDetailsSelector_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        (v4/*: any*/)
      ],
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "myLocations",
      "plural": true,
      "selections": (v3/*: any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "date",
          "variableName": "dateToGetAvailableDesks"
        },
        {
          "kind": "Variable",
          "name": "deskIdsToInclude",
          "variableName": "deskIdsToIncludeToGetAvailableDesks"
        },
        {
          "kind": "Variable",
          "name": "locationId",
          "variableName": "locationId"
        }
      ],
      "concreteType": "BookingDeskDetails",
      "kind": "LinkedField",
      "name": "availableLocationDesks",
      "plural": true,
      "selections": [
        (v5/*: any*/),
        (v2/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "BookingLocationTagDetails",
          "kind": "LinkedField",
          "name": "locationTags",
          "plural": true,
          "selections": [
            (v5/*: any*/),
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
        }
      ],
      "storageKey": null
    },
    {
      "alias": "bookingDetailsSelectorQueryPaginatedOrganizationMembers",
      "args": [
        {
          "kind": "Variable",
          "name": "orderBy",
          "variableName": "bookingDetailsSelectorOrganizationMembersSortingValues"
        },
        {
          "fields": [
            {
              "kind": "Variable",
              "name": "nameContains",
              "variableName": "bookingPeopleNameSearchText"
            },
            (v4/*: any*/)
          ],
          "kind": "ObjectValue",
          "name": "where"
        }
      ],
      "concreteType": "OrganizationMemberConnection",
      "kind": "LinkedField",
      "name": "__bookingDetailsSelectorQuery_bookingDetailsSelectorQueryPaginatedOrganizationMembers_connection",
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
          "concreteType": "OrganizationMemberEdge",
          "kind": "LinkedField",
          "name": "edges",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationMemberDetails",
              "kind": "LinkedField",
              "name": "node",
              "plural": false,
              "selections": [
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "OrganizationCustomerDetails",
                  "kind": "LinkedField",
                  "name": "customer",
                  "plural": false,
                  "selections": [
                    (v5/*: any*/),
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

(node as any).hash = "b803572cc2b8c1116a0e7f9503c933ea";

export default node;
