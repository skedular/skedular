/**
 * @generated SignedSource<<0590eb2ed2130a00798f9f0e8c833db6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingDetailsSelector_availableLocationDesks_query$data = {
  readonly availableLocationDesks?: ReadonlyArray<{
    readonly locationTags: ReadonlyArray<{
      readonly name: string;
      readonly tagType: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
  }> | null | undefined;
  readonly " $fragmentType": "bookingDetailsSelector_availableLocationDesks_query";
};
export type bookingDetailsSelector_availableLocationDesks_query$key = {
  readonly " $data"?: bookingDetailsSelector_availableLocationDesks_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_availableLocationDesks_query">;
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
};
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
      "operation": require('./bookingDetailsSelector_availableLocationDesks_refetchableFragment.graphql')
    }
  },
  "name": "bookingDetailsSelector_availableLocationDesks_query",
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
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingLocationTagDetails",
              "kind": "LinkedField",
              "name": "locationTags",
              "plural": true,
              "selections": [
                (v0/*: any*/),
                (v1/*: any*/),
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
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "763e9cf8411b3f953b8d4b6da9b2455b";

export default node;
