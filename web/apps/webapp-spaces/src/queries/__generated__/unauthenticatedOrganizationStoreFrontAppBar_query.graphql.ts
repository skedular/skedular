/**
 * @generated SignedSource<<3af420f115921d6531665a3e92f2f3fd>>
 * @lightSyntaxTransform
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
      "name": "organizationCustomDomain"
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
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
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

(node as any).hash = "f021dd93b6a20a61dc18e739f195d47c";

export default node;
