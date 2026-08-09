/**
 * @generated SignedSource<<7e3ac4725566681b1fbd34ed120926da>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type modifyMarketplaceBookingDialog_query$data = {
  readonly booking: {
    readonly marketplaceBookingResourceSelection: {
      readonly availableResourceIds: ReadonlyArray<string>;
      readonly canSelectResources: boolean;
      readonly eligibleLocations: ReadonlyArray<{
        readonly name: string;
        readonly uniqueId: string;
      }>;
      readonly eligibleResources: ReadonlyArray<{
        readonly resource: {
          readonly id: string;
          readonly name: string;
        };
      }>;
      readonly maximumResourceCount: number;
    };
  } | null | undefined;
  readonly " $fragmentType": "modifyMarketplaceBookingDialog_query";
};
export type modifyMarketplaceBookingDialog_query$key = {
  readonly " $data"?: modifyMarketplaceBookingDialog_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"modifyMarketplaceBookingDialog_query">;
};

import modifyMarketplaceBookingDialog_booking_refetchableFragment_graphql from './modifyMarketplaceBookingDialog_booking_refetchableFragment.graphql';

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "bookingId"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "from"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "locationId"
    },
    {
      "defaultValue": null,
      "kind": "LocalArgument",
      "name": "until"
    }
  ],
  "kind": "Fragment",
  "metadata": {
    "refetch": {
      "connection": null,
      "fragmentPathInResult": [],
      "operation": modifyMarketplaceBookingDialog_booking_refetchableFragment_graphql
    }
  },
  "name": "modifyMarketplaceBookingDialog_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "bookingId"
        }
      ],
      "concreteType": "BookingDetails",
      "kind": "LinkedField",
      "name": "booking",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "from",
              "variableName": "from"
            },
            {
              "kind": "Variable",
              "name": "locationId",
              "variableName": "locationId"
            },
            {
              "kind": "Variable",
              "name": "until",
              "variableName": "until"
            }
          ],
          "concreteType": "MarketplaceBookingResourceSelectionDetails",
          "kind": "LinkedField",
          "name": "marketplaceBookingResourceSelection",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "canSelectResources",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "maximumResourceCount",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "availableResourceIds",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "Booking_LocationDetails",
              "kind": "LinkedField",
              "name": "eligibleLocations",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "uniqueId",
                  "storageKey": null
                },
                (v0/*:: as any*/)
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "BookingResourceDetails",
              "kind": "LinkedField",
              "name": "eligibleResources",
              "plural": true,
              "selections": [
                {
                  "alias": null,
                  "args": null,
                  "concreteType": "ResourceDetails",
                  "kind": "LinkedField",
                  "name": "resource",
                  "plural": false,
                  "selections": [
                    {
                      "alias": null,
                      "args": null,
                      "kind": "ScalarField",
                      "name": "id",
                      "storageKey": null
                    },
                    (v0/*:: as any*/)
                  ],
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "51b0fcb8120af9d9886747f1a2c12000";

export default node;
