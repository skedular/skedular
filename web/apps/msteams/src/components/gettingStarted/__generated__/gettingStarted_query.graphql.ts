/**
 * @generated SignedSource<<0fc2bd6e62ed1ba3aab349ef709c096c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type gettingStarted_query$data = {
  readonly organization: {
    readonly isMyOnboardingDone: boolean;
  } | null | undefined;
  readonly " $fragmentType": "gettingStarted_query";
};
export type gettingStarted_query$key = {
  readonly " $data"?: gettingStarted_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"gettingStarted_query">;
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
  "name": "gettingStarted_query",
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
          "name": "isMyOnboardingDone",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "fc570ee0f98e18dc5a780f35d724b56b";

export default node;
