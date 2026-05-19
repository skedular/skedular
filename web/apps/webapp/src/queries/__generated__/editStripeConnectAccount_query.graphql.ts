/**
 * @generated SignedSource<<9ec05a74a76b4722ca55f0b23ddf69be>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type editStripeConnectAccount_query$data = {
  readonly organizationStripeConnectAccount: {
    readonly businessType: string | null | undefined;
    readonly chargesEnabled: boolean;
    readonly companyName: string | null | undefined;
    readonly contactEmail: string | null | undefined;
    readonly contactPhone: string | null | undefined;
    readonly country: string | null | undefined;
    readonly defaultCurrency: string | null | undefined;
    readonly detailsSubmitted: boolean;
    readonly id: string;
    readonly isAuthorized: boolean;
    readonly isOnboardingCompleted: boolean;
    readonly name: string;
    readonly onboardingUrl: string;
    readonly payoutsEnabled: boolean;
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
          "name": "chargesEnabled",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "payoutsEnabled",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "detailsSubmitted",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isAuthorized",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isOnboardingCompleted",
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};

(node as any).hash = "47973911c2df5a033e25e8ba2d95fbf8";

export default node;
