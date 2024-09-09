/**
 * @generated SignedSource<<24bcf343c5ea345a384c12456186ae24>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type rootShell_query$data = {
  readonly azureTenantAdminConsentUrl: string;
  readonly isAzureTenantInstalled: boolean;
  readonly me: {
    readonly id: string;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"observability_query">;
  readonly " $fragmentType": "rootShell_query";
};
export type rootShell_query$key = {
  readonly " $data"?: rootShell_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"rootShell_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "rootShell_query",
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
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "isAzureTenantInstalled",
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "kind": "ScalarField",
      "name": "azureTenantAdminConsentUrl",
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "observability_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "55d7aeae55c9449169036c0fbea878e4";

export default node;
