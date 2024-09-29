/**
 * @generated SignedSource<<98b4b752f45b25dd1c97ec2abf4df6a1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationPage_query$data = {
  readonly location: {
    readonly canViewAnalytics: boolean;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"locationAboutTab_query" | "locationBookingsTab_query" | "locationDesksTab_query" | "locationPeopleTab_query" | "locationPeopleTab_query_organizationMembers" | "locationZonesTab_query">;
  readonly " $fragmentType": "locationPage_query";
};
export type locationPage_query$key = {
  readonly " $data"?: locationPage_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationPage_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "locationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "locationPage_query",
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
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canViewAnalytics",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationBookingsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationAboutTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationPeopleTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationPeopleTab_query_organizationMembers"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationZonesTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationDesksTab_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "38c7d10ed2f62c5690d2284886767aad";

export default node;
