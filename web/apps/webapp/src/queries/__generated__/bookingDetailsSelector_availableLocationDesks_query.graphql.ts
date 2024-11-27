/**
 * @generated SignedSource<<4501a113cc9dfbe79c7ae83adff9bdf1>>
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
    readonly deskTypes: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
    readonly name: string;
    readonly uniqueId: string;
    readonly zones: ReadonlyArray<{
      readonly name: string;
      readonly uniqueId: string;
    }>;
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
              "concreteType": "BookingOrganizationDeskTypeDetails",
              "kind": "LinkedField",
              "name": "deskTypes",
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

(node as any).hash = "d0a4117b6ac7364783b2a009a02138f3";

export default node;
