/**
 * @generated SignedSource<<68a73b0c18ba88d7ad9c303a274d79f0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newBookingDialog_availableLocationRooms_refetchableFragment$variables = {
  dateToGetAvailableRooms: any;
  locationExists: boolean;
  locationId?: string | null | undefined;
};
export type newBookingDialog_availableLocationRooms_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_availableLocationRooms_query">;
};
export type newBookingDialog_availableLocationRooms_refetchableFragment = {
  response: newBookingDialog_availableLocationRooms_refetchableFragment$data;
  variables: newBookingDialog_availableLocationRooms_refetchableFragment$variables;
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
    "name": "newBookingDialog_availableLocationRooms_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "newBookingDialog_availableLocationRooms_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "newBookingDialog_availableLocationRooms_refetchableFragment",
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
    "cacheID": "0dd7b281355c6aba3c58974c834a07cf",
    "id": null,
    "metadata": {},
    "name": "newBookingDialog_availableLocationRooms_refetchableFragment",
    "operationKind": "query",
    "text": "query newBookingDialog_availableLocationRooms_refetchableFragment(\n  $dateToGetAvailableRooms: DateTime!\n  $locationExists: Boolean!\n  $locationId: String\n) {\n  ...newBookingDialog_availableLocationRooms_query\n}\n\nfragment newBookingDialog_availableLocationRooms_query on Query {\n  availableRooms(where: {locationId: $locationId, date: $dateToGetAvailableRooms}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n      color\n    }\n    zones {\n      uniqueId\n      name\n      color\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "6304bea904b99c2ba57460c8735b3329";

export default node;
