/**
 * @generated SignedSource<<2d1e8b46f5704d4ae28e9805a62b545e>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneCard_Query$data = {
  readonly location: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly me: {
    readonly id: string;
    readonly preferredZones: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly " $fragmentType": "zoneCard_Query";
};
export type zoneCard_Query$key = {
  readonly " $data"?: zoneCard_Query$data;
  readonly " $fragmentSpreads": FragmentRefs<"zoneCard_Query">;
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
  "name": "zoneCard_Query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
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
          "concreteType": "CustomerLocationTagDetails",
          "kind": "LinkedField",
          "name": "preferredZones",
          "plural": true,
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "e3009bea4cac5e32906d4e31c1f53c11";

export default node;
