/**
 * @generated SignedSource<<5f2cefbc7cce4bf0894a0f58f41ddfc8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { Fragment, ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type bookingSingleChoiceOrganization_query$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
  }>;
  readonly " $fragmentType": "bookingSingleChoiceOrganization_query";
};
export type bookingSingleChoiceOrganization_query$key = {
  readonly " $data"?: bookingSingleChoiceOrganization_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"bookingSingleChoiceOrganization_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "bookingSingleChoiceOrganization_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
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
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "758a3b8c5bb204b5ca63884ec9d7a8af";

export default node;
