/**
 * @generated SignedSource<<56975d7fa76b98e83ae4603ba2fe1a0e>>
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
export type myLocations_locations_refetchableFragment$variables = {
  locationsSortingValues?: ReadonlyArray<LocationOrderInput> | null | undefined;
  organizationId: string;
  todayDate: any;
};
export type myLocations_locations_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"myLocations_locations_query">;
};
export type myLocations_locations_refetchableFragment = {
  response: myLocations_locations_refetchableFragment$data;
  variables: myLocations_locations_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
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
  }
],
v1 = {
  "kind": "Variable",
  "name": "organizationId",
  "variableName": "organizationId"
};
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "myLocations_locations_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "myLocations_locations_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "myLocations_locations_refetchableFragment",
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
              (v1/*: any*/)
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
                    "kind": "ScalarField",
                    "name": "name",
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
    ]
  },
  "params": {
    "cacheID": "fbea43e2eedb2f4d49f4716811b32a4f",
    "id": null,
    "metadata": {},
    "name": "myLocations_locations_refetchableFragment",
    "operationKind": "query",
    "text": "query myLocations_locations_refetchableFragment(\n  $locationsSortingValues: [LocationOrderInput!]\n  $organizationId: String!\n  $todayDate: DateTime!\n) {\n  ...myLocations_locations_query\n}\n\nfragment myLocations_locations_query on Query {\n  locations(where: {organizationId: $organizationId}, orderBy: $locationsSortingValues) {\n    totalCount\n    edges {\n      node {\n        id\n        name\n      }\n    }\n  }\n  availableOrganizationDesks(organizationId: $organizationId, date: $todayDate, deskIdsToInclude: []) {\n    location {\n      uniqueId\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "2d606e61a1e7911e38354090909e9666";

export default node;
