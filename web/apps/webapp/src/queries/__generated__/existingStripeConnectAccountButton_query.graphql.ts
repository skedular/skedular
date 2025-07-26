/**
 * @generated SignedSource<<9e10f564c31dd803d9033a49fd1c367d>>
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
      "name": "organizationId"
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

(node as any).hash = "8a130fe9fec6ca47e97a400790927902";

export default node;
