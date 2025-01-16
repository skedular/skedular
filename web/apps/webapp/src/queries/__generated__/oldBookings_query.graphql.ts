/**
 * @generated SignedSource<<9ed8265d9542d2412de26752ac07732c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type oldBookings_query$data = {
  readonly location?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly organization?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly team?: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"newBookingDialog_query" | "oldBookingCard_query">;
  readonly " $fragmentType": "oldBookings_query";
};
export type oldBookings_query$key = {
  readonly " $data"?: oldBookings_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"oldBookings_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  (v0/*: any*/),
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
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
      "name": "organizationExists"
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
  "name": "oldBookings_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/)
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
              "name": "id",
              "variableName": "organizationId"
            }
          ],
          "concreteType": "OrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": (v1/*: any*/),
          "storageKey": null
        }
      ]
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
              "name": "id",
              "variableName": "locationId"
            }
          ],
          "concreteType": "LocationDetails",
          "kind": "LinkedField",
          "name": "location",
          "plural": false,
          "selections": (v1/*: any*/),
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
              "name": "id",
              "variableName": "teamId"
            }
          ],
          "concreteType": "TeamDetails",
          "kind": "LinkedField",
          "name": "team",
          "plural": false,
          "selections": (v1/*: any*/),
          "storageKey": null
        }
      ]
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "oldBookingCard_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "newBookingDialog_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "7d77d5d7c1787531c86ab478feabec76";

export default node;
