/**
 * @generated SignedSource<<14fba3332017b68a0ce7c8b647dcad1b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type LocationOrderField = "About" | "Name" | "Timezone" | "%future added value";
export type OrderDirection = "Ascending" | "Descending" | "%future added value";
export type LocationOrderInput = {
  direction: OrderDirection;
  field: LocationOrderField;
};
export type myLocations_locations_availableOrganizationDesks_refetchableFragment$variables = {
  count?: number | null | undefined;
  cursor?: string | null | undefined;
  customTagIds?: ReadonlyArray<string> | null | undefined;
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId?: string | null | undefined;
  todayDate: any;
  zoneIds?: ReadonlyArray<string> | null | undefined;
};
export type myLocations_locations_availableOrganizationDesks_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myLocations_locations_availableOrganizationDesks_query">;
};
export type myLocations_locations_availableOrganizationDesks_refetchableFragment = {
  response: myLocations_locations_availableOrganizationDesks_refetchableFragment$data;
  variables: myLocations_locations_availableOrganizationDesks_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "customTagIds"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationsSortingValues"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "organizationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "todayDate"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "zoneIds"
  }
],
v1 = {
  "kind": "Variable",
  "name": "customTagIds",
  "variableName": "customTagIds"
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
v4 = [
  {
    "kind": "Variable",
    "name": "after",
    "variableName": "cursor"
  },
  {
    "kind": "Variable",
    "name": "first",
    "variableName": "count"
  },
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
v5 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v6 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v7 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v8 = [
  (v7/*: any*/),
  (v6/*: any*/)
],
v9 = [
  (v7/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "selections": [
      {
        "args": [
          {
            "kind": "Variable",
            "name": "count",
            "variableName": "count"
          },
          {
            "kind": "Variable",
            "name": "cursor",
            "variableName": "cursor"
          }
        ],
        "kind": "FragmentSpread",
        "name": "myLocations_locations_availableOrganizationDesks_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "selections": [
      {
        "alias": null,
        "args": (v4/*: any*/),
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
                  (v5/*: any*/),
                  (v6/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "customTags",
                    "plural": true,
                    "selections": (v8/*: any*/),
                    "storageKey": null
                  },
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "Organization_OrganizationTagDetails",
                    "kind": "LinkedField",
                    "name": "zones",
                    "plural": true,
                    "selections": (v8/*: any*/),
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
                      (v5/*: any*/)
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
                    "selections": (v9/*: any*/),
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
      },
      {
        "alias": null,
        "args": (v4/*: any*/),
        "filters": [
          "where",
          "orderBy"
        ],
        "handle": "connection",
        "key": "myLocations_locations",
        "kind": "LinkedHandle",
        "name": "locations"
      },
      {
        "alias": null,
        "args": [
          {
            "fields": [
              {
                "kind": "Literal",
                "name": "combineCustomTagsZones",
                "value": true
              },
              (v1/*: any*/),
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
            "selections": (v9/*: any*/),
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "430c2dc8e3f8148f1e693c2e4ea263ca",
    "id": null,
    "metadata": {},
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "operationKind": "query",
    "text": "query myLocations_locations_availableOrganizationDesks_refetchableFragment(\n  $count: Int = null\n  $cursor: String\n  $customTagIds: [String!]\n  $locationsSortingValues: [LocationOrderInput!]\n  $organizationId: String\n  $todayDate: DateTime!\n  $zoneIds: [String!]\n) {\n  ...myLocations_locations_availableOrganizationDesks_query_1G22uz\n}\n\nfragment myLocationCard_LocationDetails on LocationDetails {\n  id\n  name\n  customTags {\n    uniqueId\n    name\n  }\n  zones {\n    uniqueId\n    name\n  }\n  desks {\n    id\n  }\n  physicalAddress {\n    formattedAddress\n  }\n  hasFutureBooking\n  canModify\n  canDelete\n  organization {\n    uniqueId\n  }\n}\n\nfragment myLocations_locations_availableOrganizationDesks_query_1G22uz on Query {\n  locations(first: $count, after: $cursor, where: {organizationId: $organizationId, zoneIds: $zoneIds, customTagIds: $customTagIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        customTags {\n          uniqueId\n          name\n        }\n        zones {\n          uniqueId\n          name\n        }\n        desks {\n          id\n        }\n        physicalAddress {\n          formattedAddress\n        }\n        hasFutureBooking\n        canModify\n        canDelete\n        organization {\n          uniqueId\n        }\n        ...myLocationCard_LocationDetails\n        __typename\n      }\n      cursor\n    }\n    pageInfo {\n      endCursor\n      hasNextPage\n    }\n  }\n  availableDesks(where: {organizationId: $organizationId, date: $todayDate, deskIdsToInclude: [], zoneIds: $zoneIds, customTagIds: $customTagIds, combineCustomTagsZones: true}) {\n    location {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "de3633ee692c24f507ca5d4d32d8a8bf";

export default node;
