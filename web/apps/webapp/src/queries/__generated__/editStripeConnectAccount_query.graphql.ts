/**
 * @generated SignedSource<<4a060fd9861def3207929d25ed1dee36>>
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
    readonly businessType: string;
    readonly companyName: string;
    readonly country: string;
    readonly defaultCurrency: string;
    readonly email: string;
    readonly id: string;
    readonly name: string;
    readonly onboardingCompleted: boolean;
    readonly onboardingUrl: string;
    readonly phone: string;
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
          "name": "email",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "phone",
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

(node as any).hash = "bbe95348e2d2a16608b3b9b1ec7eabd4";

export default node;
