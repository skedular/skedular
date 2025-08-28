/**
 * @generated SignedSource<<da43547be26e03810f4600b07bc4061e>>
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
      "name": "organizationUniqueAlphanumericName"
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

(node as any).hash = "45adf4c88164117cca48765e751954c3";

export default node;
