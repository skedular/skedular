/**
 * @generated SignedSource<<d6fc908f6e3dc1d6e1070bf369843100>>
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
  deskTypeIds?: ReadonlyArray<string> | null | undefined;
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
    "name": "deskTypeIds"
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
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "selections": [
      {
        "args": null,
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
            "selections": [
              (v6/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "391d10185e5469616955c433823ba984",
    "id": null,
    "metadata": {},
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "operationKind": "query",
    "text": "query myLocations_locations_availableOrganizationDesks_refetchableFragment(\n  $deskTypeIds: [String!]\n  $locationsSortingValues: [LocationOrderInput!]\n  $organizationId: String\n  $todayDate: DateTime!\n  $zoneIds: [String!]\n) {\n  ...myLocations_locations_availableOrganizationDesks_query\n}\n\nfragment myLocations_locations_availableOrganizationDesks_query on Query {\n  locations(where: {organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        deskTypes {\n          uniqueId\n          name\n        }\n        zones {\n          uniqueId\n          name\n        }\n        desks {\n          id\n        }\n        physicalAddress {\n          formattedAddress\n        }\n      }\n    }\n  }\n  availableDesks(where: {organizationId: $organizationId, date: $todayDate, deskIdsToInclude: [], zoneIds: $zoneIds, deskTypeIds: $deskTypeIds, combineDeskTypesZones: true}) {\n    location {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9025cee5ba325270ac15144a037a3f6c";

export default node;
