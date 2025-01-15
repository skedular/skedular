/**
 * @generated SignedSource<<613c541b6640e636ed2383a708552567>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editBooking_availableLocationDesks_query$data = {
  readonly availableDesks?: ReadonlyArray<{
    readonly customTags: ReadonlyArray<{
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
    readonly zones: ReadonlyArray<{
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  }> | null | undefined;
  readonly " $fragmentType": "editBooking_availableLocationDesks_query";
};
export type editBooking_availableLocationDesks_query$key = {
  readonly " $data"?: editBooking_availableLocationDesks_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editBooking_availableLocationDesks_query">;
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
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
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
      "operation": require('./editBooking_availableLocationDesks_refetchableFragment.graphql')
    }
  },
  "name": "editBooking_availableLocationDesks_query",
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

(node as any).hash = "5aed9525f3ecb23eb65167e1a646f7e4";

export default node;
