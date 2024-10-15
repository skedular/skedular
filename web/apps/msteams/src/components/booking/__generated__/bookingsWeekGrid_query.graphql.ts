/**
 * @generated SignedSource<<67e42b63a6a17c08b394a7bedfb8bc86>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingsWeekGrid_query$data = {
  readonly locationBookingPermissions?: {
    readonly canAddBookingOnBehalf: boolean;
    readonly canDeleteBookingOnBehalf: boolean;
  } | null | undefined;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly organizationBookingPermissions: {
    readonly canAddBookingOnBehalf: boolean;
    readonly canDeleteBookingOnBehalf: boolean;
  } | null | undefined;
  readonly teamBookingPermissions?: {
    readonly canAddBookingOnBehalf: boolean;
    readonly canDeleteBookingOnBehalf: boolean;
  } | null | undefined;
  readonly " $fragmentType": "bookingsWeekGrid_query";
};
export type bookingsWeekGrid_query$key = {
  readonly " $data"?: bookingsWeekGrid_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingsWeekGrid_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "canAddBookingOnBehalf",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "canDeleteBookingOnBehalf",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationExists"
    },
    {
      "kind": "RootArgument",
      "name": "locationId"
    },
    {
      "kind": "RootArgument",
      "name": "organizationId"
    },
    {
      "kind": "RootArgument",
      "name": "teamExists"
    },
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingsWeekGrid_query",
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
      "selections": (v0/*: any*/),
      "storageKey": null
    },
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
              "name": "locationId",
              "variableName": "locationId"
            }
          ],
          "concreteType": "LocationBookingPermissions",
          "kind": "LinkedField",
          "name": "locationBookingPermissions",
          "plural": false,
          "selections": (v0/*: any*/),
          "storageKey": null
        }
      ]
    },
    {
      "condition": "teamExists",
      "kind": "Condition",
      "passingValue": true,
      "selections": [
        {
          "alias": null,
          "args": [
            {
              "kind": "Variable",
              "name": "teamId",
              "variableName": "teamId"
            }
          ],
          "concreteType": "TeamBookingPermissions",
          "kind": "LinkedField",
          "name": "teamBookingPermissions",
          "plural": false,
          "selections": (v0/*: any*/),
          "storageKey": null
        }
      ]
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "886b4f131c05e4f78e9b0513462df129";

export default node;
