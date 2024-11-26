/**
 * @generated SignedSource<<cb94925d6e3b835e2850eff0823bf9c6>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type zoneCard_Query$data = {
  readonly me: {
    readonly id: string;
    readonly preferredZones: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly organization: {
    readonly canModify: boolean;
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
      "name": "organizationId"
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
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
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

(node as any).hash = "d40318d5f86a1fb10b80bbb99245df67";

export default node;
