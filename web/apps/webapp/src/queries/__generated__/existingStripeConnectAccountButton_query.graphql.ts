/**
 * @generated SignedSource<<711dd24eea39ce21f6a1e3fabc814d7d>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type existingStripeConnectAccountButton_query$data = {
  readonly organization: {
    readonly stripeAuthorizeExistingConnectAccountUrl: string;
  } | null | undefined;
  readonly " $fragmentType": "existingStripeConnectAccountButton_query";
};
export type existingStripeConnectAccountButton_query$key = {
  readonly " $data"?: existingStripeConnectAccountButton_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"existingStripeConnectAccountButton_query">;
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
  "name": "existingStripeConnectAccountButton_query",
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
      "concreteType": "OrganizationDetails",
      "kind": "LinkedField",
      "name": "organization",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "stripeAuthorizeExistingConnectAccountUrl",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "9a3cad40476b4057b406abf6e29398f3";

export default node;
