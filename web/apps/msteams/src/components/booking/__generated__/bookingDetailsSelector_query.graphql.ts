/**
 * @generated SignedSource<<604daf1b265383d15fd05915cf41c88a>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingDetailsSelector_query$data = {
  readonly myLocations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly " $fragmentType": "bookingDetailsSelector_query";
};
export type bookingDetailsSelector_query$key = {
  readonly " $data"?: bookingDetailsSelector_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "id",
    "storageKey": null
  },
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
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingDetailsSelector_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": (v0/*: any*/),
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
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "myLocations",
      "plural": true,
      "selections": (v0/*: any*/),
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "3a40173705e449bc3e81acee69e3b1be";

export default node;
