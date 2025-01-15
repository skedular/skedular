/**
 * @generated SignedSource<<6ee1437fa2074e838ae0ed3ce0861b3a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editDesk_query$data = {
  readonly desk: {
    readonly customTags: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
    readonly deactivated: boolean;
    readonly id: string;
    readonly name: string;
    readonly requireBookingApproval: boolean;
    readonly zones: ReadonlyArray<{
      readonly color: string | null | undefined;
      readonly name: string | null | undefined;
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"multipleChoicesCustomTags_query" | "multipleChoicesZones_query">;
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
  (v0/*: any*/),
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
      "name": "desk",
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
          "name": "customTags",
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
      "name": "multipleChoicesCustomTags_query"
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

(node as any).hash = "c5915399a3b376558f9a682fd66bb735";

export default node;
