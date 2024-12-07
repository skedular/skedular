/**
 * @generated SignedSource<<6f9a8295869e74557184cd42452f2146>>
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
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }> | null | undefined;
  readonly myTeams: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  }> | null | undefined;
  readonly " $fragmentType": "bookingDetailsSelector_query";
};
export type bookingDetailsSelector_query$key = {
  readonly " $data"?: bookingDetailsSelector_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingDetailsSelector_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
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
  {
    "kind": "Variable",
    "name": "organizationId",
    "variableName": "nullableOrganizationId"
  }
],
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "uniqueId",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "nullableOrganizationId"
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
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": (v2/*: any*/),
      "concreteType": "LocationDetails",
      "kind": "LinkedField",
      "name": "myLocations",
      "plural": true,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationOrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": (v3/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": (v2/*: any*/),
      "concreteType": "TeamDetails",
      "kind": "LinkedField",
      "name": "myTeams",
      "plural": true,
      "selections": [
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamOrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": (v3/*: any*/),
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

(node as any).hash = "731b2eef7810f549b4ec68706d2223c4";

export default node;
