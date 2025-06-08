/**
 * @generated SignedSource<<eeb70a064337e36c97ae71e174585824>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type TeamMemberRole = "ADMINISTRATOR" | "MEMBER" | "OWNER" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type organizationTeam_query$data = {
  readonly team: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly primaryFeatureImageUrl: string | null | undefined;
    readonly primaryLocation: {
      readonly name: string;
      readonly uniqueId: string;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly teamMemberRoles: ReadonlyArray<TeamMemberRole>;
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
          "kind": "ScalarField",
          "name": "primaryFeatureImageUrl",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "Team_LocationDetails",
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
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "teamMemberRoles",
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

(node as any).hash = "ef35ebc51f63b4345ab959fcfef468a1";

export default node;
