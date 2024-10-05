/**
 * @generated SignedSource<<6edd7e359e1f95eab12502af55ce1957>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type teamPage_query$data = {
  readonly team: {
    readonly name: string;
    readonly organization: {
      readonly uniqueId: string;
    } | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"teamAboutTab_query" | "teamBookingsTab_query" | "teamPeopleTab_query">;
  readonly " $fragmentType": "teamPage_query";
};
export type teamPage_query$key = {
  readonly " $data"?: teamPage_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"teamPage_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "teamId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "teamPage_query",
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
          "name": "name",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "TeamOrganizationDetails",
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
      "name": "teamBookingsTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamAboutTab_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "teamPeopleTab_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "9ee71b5c2be781abf695bde754ae13d4";

export default node;
