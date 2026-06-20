/**
 * @generated SignedSource<<9c84967d6cc85b09ecd8628aad086437>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type noOrganizationLandingContent_query$data = {
  readonly myOrganizations: ReadonlyArray<{
    readonly customDomain: string | null | undefined;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly uniqueId: string;
  }>;
  readonly " $fragmentType": "noOrganizationLandingContent_query";
};
export type noOrganizationLandingContent_query$key = {
  readonly " $data"?: noOrganizationLandingContent_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"noOrganizationLandingContent_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "noOrganizationLandingContent_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Literal",
          "name": "types",
          "value": [
            "HOST"
          ]
        }
      ],
      "concreteType": "MyOrganizationDetails",
      "kind": "LinkedField",
      "name": "myOrganizations",
      "plural": true,
      "selections": [
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
          "name": "uniqueId",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "customDomain",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "logoUrl",
          "storageKey": null
        }
      ],
      "storageKey": "myOrganizations(types:[\"HOST\"])"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "461dbf708574975ce582cd1cdd6a9ccc";

export default node;
