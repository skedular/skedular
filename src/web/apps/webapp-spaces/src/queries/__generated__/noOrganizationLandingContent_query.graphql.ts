/**
 * @generated SignedSource<<15911236c64ca81d3e15de1340e1296d>>
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
            "MARKETPLACE"
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
      "storageKey": "myOrganizations(types:[\"MARKETPLACE\"])"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "b8ab048fb309bfbcfeba3b0d0bd36151";

export default node;
