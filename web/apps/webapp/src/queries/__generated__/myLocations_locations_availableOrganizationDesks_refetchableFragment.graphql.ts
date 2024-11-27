/**
 * @generated SignedSource<<4e03cb4da8ed25de14b050fd70eefb6b>>
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
  organizationId: string;
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
  "name": "organizationId",
  "variableName": "organizationId"
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v4 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v5 = [
  (v4/*: any*/),
  (v3/*: any*/)
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
              {
                "kind": "Variable",
                "name": "deskTypeIds",
                "variableName": "deskTypeIds"
              },
              (v1/*: any*/),
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
                  (v2/*: any*/),
                  (v3/*: any*/),
                  {
                    "alias": null,
                    "args": null,
                    "concreteType": "DeskDetails",
                    "kind": "LinkedField",
                    "name": "desks",
                    "plural": true,
                    "selections": [
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Organization_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "deskTypes",
                        "plural": true,
                        "selections": (v5/*: any*/),
                        "storageKey": null
                      },
                      {
                        "alias": null,
                        "args": null,
                        "concreteType": "Organization_OrganizationTagDetails",
                        "kind": "LinkedField",
                        "name": "zones",
                        "plural": true,
                        "selections": (v5/*: any*/),
                        "storageKey": null
                      },
                      (v2/*: any*/)
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
          (v1/*: any*/)
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
              (v4/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "88e2289a4654ba446ccf3b242126239c",
    "id": null,
    "metadata": {},
    "name": "myLocations_locations_availableOrganizationDesks_refetchableFragment",
    "operationKind": "query",
    "text": "query myLocations_locations_availableOrganizationDesks_refetchableFragment(\n  $deskTypeIds: [String!]\n  $locationsSortingValues: [LocationOrderInput!]\n  $organizationId: String!\n  $todayDate: DateTime!\n  $zoneIds: [String!]\n) {\n  ...myLocations_locations_availableOrganizationDesks_query\n}\n\nfragment myLocations_locations_availableOrganizationDesks_query on Query {\n  locations(where: {organizationId: $organizationId, zoneIds: $zoneIds, deskTypeIds: $deskTypeIds}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n        desks {\n          deskTypes {\n            uniqueId\n            name\n          }\n          zones {\n            uniqueId\n            name\n          }\n          id\n        }\n        physicalAddress {\n          formattedAddress\n        }\n      }\n    }\n  }\n  availableOrganizationDesks(organizationId: $organizationId, date: $todayDate, deskIdsToInclude: []) {\n    location {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "520a7ba3049eba3702d671d388e54eba";

export default node;
