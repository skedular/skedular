/**
 * @generated SignedSource<<c13389f6f16d75ec5e0793385b2976ef>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type newBookingDialog_query$data = {
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly organizationBookingPermissions?: {
    readonly canAddBookingOnBehalf: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_availableLocationDesks_query" | "bookingDetailsSelector_organizationMembers_query" | "bookingDetailsSelector_query">;
  readonly " $fragmentType": "newBookingDialog_query";
};
export type newBookingDialog_query$key = {
  readonly " $data"?: newBookingDialog_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationExists"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "newBookingDialog_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "condition": "organizationExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "organizationId",
              "variableName": "organizationId"
            }
          ],
          "concreteType": "OrganizationBookingPermissions",
          "kind": "LinkedField",
          "name": "organizationBookingPermissions",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "canAddBookingOnBehalf",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ]
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_organizationMembers_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "bookingDetailsSelector_availableLocationDesks_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "c86b3efd02d24421a082c4bf5d4a1390";

export default node;
