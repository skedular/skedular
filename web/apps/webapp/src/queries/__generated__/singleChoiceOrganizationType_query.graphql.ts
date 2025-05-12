/**
 * @generated SignedSource<<08fdfb9c9834ac57aa36bbd0889510f8>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type singleChoiceOrganizationType_query$data = {
  readonly organizationTypes: ReadonlyArray<{
    readonly name: string;
    readonly type: OrganizationType;
  }>;
  readonly " $fragmentType": "singleChoiceOrganizationType_query";
};
export type singleChoiceOrganizationType_query$key = {
  readonly " $data"?: singleChoiceOrganizationType_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"singleChoiceOrganizationType_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "singleChoiceOrganizationType_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTypeDetails",
      "kind": "LinkedField",
      "name": "organizationTypes",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "type",
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

(node as any).hash = "faeca831087526d3a47d99e269a5d733";

export default node;
