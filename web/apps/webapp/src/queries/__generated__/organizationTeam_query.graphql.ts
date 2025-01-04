/**
 * @generated SignedSource<<9df46b51f04e0287dc7ad28ecfe48230>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationTeam_query$data = {
  readonly team: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly primaryLocation: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceLocation_locations_query">;
  readonly " $fragmentType": "organizationTeam_query";
};
export type organizationTeam_query$key = {
  readonly " $data"?: organizationTeam_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationTeam_query">;
};

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
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationTeam_query",
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
          "name": "about",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "timezone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamLocationDetails",
          "kind": "LinkedField",
          "name": "primaryLocation",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
              "storageKey": null
            },
            (v0/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceLocation_locations_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "11af04e6cd7312d9dd6dd34612a920bc";

export default node;
