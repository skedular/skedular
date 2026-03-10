/**
 * @generated SignedSource<<886b66d1371e0c1e663fe95c40f38a1f>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type unauthenticatedOrganizationStoreFrontAppBar_query$data = {
  readonly organizationPublic: {
    readonly name: string;
  } | null | undefined;
  readonly " $fragmentType": "unauthenticatedOrganizationStoreFrontAppBar_query";
};
export type unauthenticatedOrganizationStoreFrontAppBar_query$key = {
  readonly " $data"?: unauthenticatedOrganizationStoreFrontAppBar_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"unauthenticatedOrganizationStoreFrontAppBar_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationUniqueAlphanumericName"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "unauthenticatedOrganizationStoreFrontAppBar_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "uniqueAlphanumericName",
          "variableName": "organizationUniqueAlphanumericName"
        }
      ],
      "concreteType": "OrganizationPublicDetails",
      "kind": "LinkedField",
      "name": "organizationPublic",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "name",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "db94251da148bc3d9d2f3db1c85b0dc1";

export default node;
