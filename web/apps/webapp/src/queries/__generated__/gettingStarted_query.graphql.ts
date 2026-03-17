/**
 * @generated SignedSource<<104032ad056d794c9dac98995e36c275>>
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
      "name": "organizationCustomDomain"
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

(node as any).hash = "7540897b6a00ec1096da23e3efb82ef1";

export default node;
