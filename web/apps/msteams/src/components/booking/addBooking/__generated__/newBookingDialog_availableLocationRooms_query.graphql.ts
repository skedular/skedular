/**
 * @generated SignedSource<<c63bf42b6e8b4829daec6a1df6fb2d2f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newBookingDialog_availableLocationRooms_query$data = {
  readonly availableRooms?: ReadonlyArray<{
    readonly customTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
    readonly zones: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  }> | null | undefined;
  readonly " $fragmentType": "newBookingDialog_availableLocationRooms_query";
};
export type newBookingDialog_availableLocationRooms_query$key = {
  readonly " $data"?: newBookingDialog_availableLocationRooms_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_availableLocationRooms_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "uniqueId",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "color",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "dateToGetAvailableRooms"
    },
    {
      "kind": "RootArgument",
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": require('./newBookingDialog_availableLocationRooms_refetchableFragment.graphql')
    }
  },
  "name": "newBookingDialog_availableLocationRooms_query",
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
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingOrganizationCustomTagDetails",
              "kind": "LinkedField",
              "name": "customTags",
              "plural": true,
              "selections": (v2/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingOrganizationZoneDetails",
              "kind": "LinkedField",
              "name": "zones",
              "plural": true,
              "selections": (v2/*: any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6304bea904b99c2ba57460c8735b3329";

export default node;
