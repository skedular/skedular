/**
 * @generated SignedSource<<3312972ac5f6c5f07cae64b1e79d23df>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editStripeConnectAccount_query$data = {
  readonly organizationStripeConnectAccount: {
    readonly businessType: string | null | undefined;
    readonly companyName: string | null | undefined;
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly country: string | null | undefined;
    readonly defaultCurrency: string | null | undefined;
    readonly id: string;
    readonly name: string;
    readonly onboardingCompleted: boolean;
    readonly onboardingUrl: string;
    readonly supportUrl: string | null | undefined;
    readonly url: string | null | undefined;
  } | null | undefined;
  readonly " $fragmentType": "editStripeConnectAccount_query";
};
export type editStripeConnectAccount_query$key = {
  readonly " $data"?: editStripeConnectAccount_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"editStripeConnectAccount_query">;
};

const node: ReaderFragment = {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationStripeConnectAccountId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "editStripeConnectAccount_query",
  "selections": [
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "id",
          "variableName": "organizationStripeConnectAccountId"
        }
      ],
      "concreteType": "OrganizationStripeConnectAccountDetails",
      "kind": "LinkedField",
      "name": "organizationStripeConnectAccount",
      "plural": false,
      "selections": [
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "id",
          "storageKey": null
        },
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
          "name": "country",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "defaultCurrency",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "businessType",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "companyName",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "url",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "supportUrl",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactEmail",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "contactPhone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "onboardingUrl",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "onboardingCompleted",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "6c923cb8157d12b69fc86cf40274c99a";

export default node;
