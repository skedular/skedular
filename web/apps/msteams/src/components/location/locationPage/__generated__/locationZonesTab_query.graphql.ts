/**
 * @generated SignedSource<<4c35e1de3fcaaae819acb6a81ddd43b0>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type locationZonesTab_query$data = {
  readonly location: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_Query">;
  readonly " $fragmentType": "locationZonesTab_query";
};
export type locationZonesTab_query$key = {
  readonly " $data"?: locationZonesTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"locationZonesTab_query">;
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
  "name": "locationZonesTab_query",
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
          "name": "canModify",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "zoneCard_Query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "c099caa8b204fc3b78001cdcdf187811";

export default node;
