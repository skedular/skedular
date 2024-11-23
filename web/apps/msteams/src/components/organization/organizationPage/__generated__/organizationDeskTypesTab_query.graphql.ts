/**
 * @generated SignedSource<<8b09864ba2f6420e0ba1372ada27b3d2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationDeskTypesTab_query$data = {
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"deskTypeCard_Query">;
  readonly " $fragmentType": "organizationDeskTypesTab_query";
};
export type organizationDeskTypesTab_query$key = {
  readonly " $data"?: organizationDeskTypesTab_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationDeskTypesTab_query">;
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
  "name": "organizationDeskTypesTab_query",
  "selections": [
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
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "deskTypeCard_Query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "86cebb386882c507917282e1779f6fe2";

export default node;
