/**
 * @generated SignedSource<<78fc7a8c3ac90f2011479d6a37bf9109>>
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
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
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
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "LocationOrganizationDetails",
          "kind": "LinkedField",
          "name": "organization",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "uniqueId",
              "storageKey": null
            }
          ],
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

(node as any).hash = "05119516fe2f8ca2e3e10cfc3bdadd01";

export default node;
