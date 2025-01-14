/**
 * @generated SignedSource<<f94a1ca9f1cf43bdee424414ccdb940b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type customTagCard_Query$data = {
  readonly me: {
    readonly id: string;
    readonly preferredCustomTags: ReadonlyArray<{
      readonly uniqueId: string;
    }>;
  } | null | undefined;
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentType": "customTagCard_Query";
};
export type customTagCard_Query$key = {
  readonly " $data"?: customTagCard_Query$data;
  readonly " $fragmentSpreads": FragmentRefs<"customTagCard_Query">;
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
  "name": "customTagCard_Query",
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
          "concreteType": "CustomerOrganizationTagDetails",
          "kind": "LinkedField",
          "name": "preferredCustomTags",
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

(node as any).hash = "e026493fadfb5a7d2319b1316e6db280";

export default node;
