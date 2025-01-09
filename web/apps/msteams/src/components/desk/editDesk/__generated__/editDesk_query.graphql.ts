/**
 * @generated SignedSource<<5bf95a4bf916c7e5d275ef91a80abf25>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editDesk_query$data = {
  readonly locationDesk: {
    readonly deactivated: boolean;
    readonly deskTypes: ReadonlyArray<{
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly id: string;
    readonly name: string;
    readonly requireBookingApproval: boolean;
    readonly zones: ReadonlyArray<{
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesDeskTypes_query" | "multipleChoicesZones_query">;
  readonly " $fragmentType": "editDesk_query";
};
export type editDesk_query$key = {
  readonly " $data"?: editDesk_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editDesk_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v1 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  },
  (v0/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "deskId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editDesk_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "deskId"
        }
      ],
      "concreteType": "DeskDetails",
      "kind": "LinkedField",
      "name": "locationDesk",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        },
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "deactivated",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "requireBookingApproval",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Organization_OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "deskTypes",
          "plural": true,
          "selections": (v1/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Organization_OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "zones",
          "plural": true,
          "selections": (v1/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesDeskTypes_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "multipleChoicesZones_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "aceb9f81a0bd42828128913327f9df33";

export default node;
