/**
 * @generated SignedSource<<b73ec62d8b4d71a9ae9935e29134c3a8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editBooking_availableLocationRooms_refetchableFragment$variables = {
  dateToGetAvailableRooms: any;
  locationExists: boolean;
  locationId?: string | null | undefined;
  roomIdsToIncludeToGetAvailableRooms?: ReadonlyArray<string> | null | undefined;
};
export type editBooking_availableLocationRooms_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"editBooking_availableLocationRooms_query">;
};
export type editBooking_availableLocationRooms_refetchableFragment = {
  response: editBooking_availableLocationRooms_refetchableFragment$data;
  variables: editBooking_availableLocationRooms_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateToGetAvailableRooms"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationExists"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "locationId"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "roomIdsToIncludeToGetAvailableRooms"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
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
  (v2/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editBooking_availableLocationRooms_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_availableLocationRooms_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editBooking_availableLocationRooms_refetchableFragment",
    "selections": [
      {
        "condition": "locationExists",
        "kind": "Condition",
        "passingValue": true,
        "selections": [
          {
            "alias": null,
            "args": [
              {
                "fields": [
                  {
                    "kind": "Variable",
                    "name": "date",
                    "variableName": "dateToGetAvailableRooms"
                  },
                  {
                    "kind": "Variable",
                    "name": "locationId",
                    "variableName": "locationId"
                  },
                  {
                    "kind": "Variable",
                    "name": "roomIdsToInclude",
                    "variableName": "roomIdsToIncludeToGetAvailableRooms"
                  }
                ],
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingRoomDetails",
            "kind": "LinkedField",
            "name": "availableRooms",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/),
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingOrganizationCustomTagDetails",
                "kind": "LinkedField",
                "name": "customTags",
                "plural": true,
                "selections": (v3/*: any*/),
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "concreteType": "BookingOrganizationZoneDetails",
                "kind": "LinkedField",
                "name": "zones",
                "plural": true,
                "selections": (v3/*: any*/),
                "storageKey": null
              }
            ],
            "storageKey": null
          }
        ]
      }
    ]
  },
  "params": {
    "cacheID": "13ad5089f75e89f7dd78580c3fb20062",
    "id": null,
    "metadata": {},
    "name": "editBooking_availableLocationRooms_refetchableFragment",
    "operationKind": "query",
    "text": "query editBooking_availableLocationRooms_refetchableFragment(\n  $dateToGetAvailableRooms: DateTime!\n  $locationExists: Boolean!\n  $locationId: String\n  $roomIdsToIncludeToGetAvailableRooms: [String!]\n) {\n  ...editBooking_availableLocationRooms_query\n}\n\nfragment editBooking_availableLocationRooms_query on Query {\n  availableRooms(where: {locationId: $locationId, date: $dateToGetAvailableRooms, roomIdsToInclude: $roomIdsToIncludeToGetAvailableRooms}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "d35cbe7ece6fdf3abd09d68843394229";

export default node;
