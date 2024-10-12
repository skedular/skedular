/**
 * @generated SignedSource<<c7023e9c1b63e080d220cee03c2b5149>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationPeopleTab_query$data = {
  readonly location: {
    readonly id: string;
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"locationSingleChoiceMembershipType_query">;
  readonly " $fragmentType": "locationPeopleTab_query";
};
export type locationPeopleTab_query$key = {
  readonly " $data"?: locationPeopleTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationPeopleTab_query">;
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
  "name": "locationPeopleTab_query",
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
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "locationSingleChoiceMembershipType_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "3dc7ddc993c77f3f04b9cd1fe6b544c7";

export default node;
