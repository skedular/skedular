/**
 * @generated SignedSource<<a050c03be5c1c148d0aa54f677a8ce1c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationLocation_query$data = {
  readonly location: {
    readonly about: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly physicalAddress: {
      readonly formattedAddress: string | null | undefined;
    } | null | undefined;
    readonly timezone: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"deskTypeSelector_allDeskTypes_query" | "zoneSelector_allZones_query">;
  readonly " $fragmentType": "organizationLocation_query";
};
export type organizationLocation_query$key = {
  readonly " $data"?: organizationLocation_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationLocation_query">;
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
  "name": "organizationLocation_query",
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
        },
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
          "concreteType": "LocationAddressDetails",
          "kind": "LinkedField",
          "name": "physicalAddress",
          "plural": false,
          "selections": [
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "formattedAddress",
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
      "name": "deskTypeSelector_allDeskTypes_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneSelector_allZones_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "db651047a82ad6d015adc8462cf4aa66";

export default node;
