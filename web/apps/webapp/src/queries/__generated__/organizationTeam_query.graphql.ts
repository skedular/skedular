/**
 * @generated SignedSource<<0b43c369cef717c0ab4c32295caea851>>
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
    readonly featureImages: ReadonlyArray<{
      readonly original: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
      readonly thumbnail: {
        readonly height: number | null | undefined;
        readonly url: string;
        readonly width: number | null | undefined;
      } | null | undefined;
    }>;
    readonly id: string;
    readonly name: string;
    readonly primaryLocation: {
      readonly id: string;
      readonly name: string;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly teamMemberRoles: ReadonlyArray<{
    readonly name: string;
    readonly type: TeamMemberRole;
  }>;
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
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "url",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "height",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "width",
    "storageKey": null
  }
];
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
        (v0/*: any*/),
        (v1/*: any*/),
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
          "concreteType": "CdnImageFile",
          "kind": "LinkedField",
          "name": "featureImages",
          "plural": true,
          "selections": [
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "original",
              "plural": false,
              "selections": (v2/*: any*/),
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "CdnFile",
              "kind": "LinkedField",
              "name": "thumbnail",
              "plural": false,
              "selections": (v2/*: any*/),
              "storageKey": null
            }
          ],
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationDetails",
          "kind": "LinkedField",
          "name": "primaryLocation",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "TeamMemberRoleDetails",
      "kind": "LinkedField",
      "name": "teamMemberRoles",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
          "storageKey": null
        },
        (v1/*: any*/)
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

(node as any).hash = "4105f45f3d79567bcde690bfb2da62e5";

export default node;
