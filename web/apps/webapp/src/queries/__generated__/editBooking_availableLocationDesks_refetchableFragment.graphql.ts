/**
 * @generated SignedSource<<3eac505bbb6008940e48f18630df8405>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editBooking_availableLocationDesks_refetchableFragment$variables = {
  dateToGetAvailableDesks: any;
  deskIdsToIncludeToGetAvailableDesks?: ReadonlyArray<string> | null | undefined;
  locationExists: boolean;
  locationId?: string | null | undefined;
};
export type editBooking_availableLocationDesks_refetchableFragment$data = {
  readonly " $fragmentSpreads": FragmentRefs<"editBooking_availableLocationDesks_query">;
};
export type editBooking_availableLocationDesks_refetchableFragment = {
  response: editBooking_availableLocationDesks_refetchableFragment$data;
  variables: editBooking_availableLocationDesks_refetchableFragment$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "dateToGetAvailableDesks"
  },
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "deskIdsToIncludeToGetAvailableDesks"
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
  (v2/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "editBooking_availableLocationDesks_refetchableFragment",
    "selections": [
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "editBooking_availableLocationDesks_query"
      }
    ],
    "type": "Query",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "editBooking_availableLocationDesks_refetchableFragment",
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
                "kind": "ObjectValue",
                "name": "where"
              }
            ],
            "concreteType": "BookingDeskDetails",
            "kind": "LinkedField",
            "name": "availableDesks",
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
    "cacheID": "a989c66d8d8024dc14d4d18a0e0e26ac",
    "id": null,
    "metadata": {},
    "name": "editBooking_availableLocationDesks_refetchableFragment",
    "operationKind": "query",
    "text": "query editBooking_availableLocationDesks_refetchableFragment(\n  $dateToGetAvailableDesks: DateTime!\n  $deskIdsToIncludeToGetAvailableDesks: [String!]\n  $locationExists: Boolean!\n  $locationId: String\n) {\n  ...editBooking_availableLocationDesks_query\n}\n\nfragment editBooking_availableLocationDesks_query on Query {\n  availableDesks(where: {locationId: $locationId, date: $dateToGetAvailableDesks, deskIdsToInclude: $deskIdsToIncludeToGetAvailableDesks}) @include(if: $locationExists) {\n    uniqueId\n    name\n    customTags {\n      uniqueId\n      name\n    }\n    zones {\n      uniqueId\n      name\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "5aed9525f3ecb23eb65167e1a646f7e4";

export default node;
