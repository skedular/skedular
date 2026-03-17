/**
 * @generated SignedSource<<fbd36f1279306c587cba328b236eaed5>>
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
      "name": "organizationCustomDomain"
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
          "name": "customDomain",
          "variableName": "organizationCustomDomain"
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

(node as any).hash = "6f024d8a27d90dd2dbdcfc39f1cce931";

export default node;
