/**
 * @generated SignedSource<<4dea26455c4b54088e1560c6aa32b827>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
export type Currency = "NZD" | "USD" | "%future added value";
export type MembershipTerm = "DAILY" | "FIVE_MONTHS" | "FORTNIGHTLY" | "FOUR_MONTHS" | "MONTHLY" | "NOT_SET" | "QUARTERLY" | "SIX_MONTHS" | "TWO_MONTHS" | "WEEKLY" | "YEARLY" | "%future added value";
import { FragmentRefs } from "relay-runtime";
export type productCard_query$data = {
  readonly currencies: ReadonlyArray<{
    readonly name: string;
    readonly type: Currency;
  }>;
  readonly membershipTerms: ReadonlyArray<{
    readonly name: string;
    readonly type: MembershipTerm;
  }>;
  readonly organization: {
    readonly canModify: boolean;
  } | null | undefined;
  readonly " $fragmentType": "productCard_query";
};
export type productCard_query$key = {
  readonly " $data"?: productCard_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"productCard_query">;
};

const node: ReaderFragment = (function(){
var v0 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "name",
    "storageKey": null
  }
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationCustomDomain"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "productCard_query",
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
          "name": "canModify",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "MembershipTermDetails",
      "kind": "LinkedField",
      "name": "membershipTerms",
      "plural": true,
      "selections": (v0/*:: as any*/),
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "CurrencyDetails",
      "kind": "LinkedField",
      "name": "currencies",
      "plural": true,
      "selections": (v0/*:: as any*/),
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "6fa99000bf4433bd69a95ea426c2961e";

export default node;
