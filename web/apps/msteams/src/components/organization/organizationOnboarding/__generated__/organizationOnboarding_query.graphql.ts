/**
 * @generated SignedSource<<babf668beeee53e62ae8e974bf298b31>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationOnboarding_query$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly me: {
    readonly id: string;
    readonly isLocationOnboardingDone: boolean;
    readonly isOrganizationOnboardingDone: boolean;
  } | null | undefined;
  readonly organizationIndustryMainCategoriesReferences: ReadonlyArray<{
    readonly subCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "organizationTermsOfUse_query">;
  readonly " $fragmentType": "organizationOnboarding_query";
};
export type organizationOnboarding_query$key = {
  readonly " $data"?: organizationOnboarding_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationOnboarding_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationOnboarding_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "CustomerDetails",
      "kind": "LinkedField",
      "name": "me",
      "plural": false,
      "selections": [
        (v0/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isOrganizationOnboardingDone",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "isLocationOnboardingDone",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationTermsOfUse",
      "kind": "LinkedField",
      "name": "activeOrganizationTermsOfUse",
      "plural": false,
      "selections": [
        (v0/*: any*/)
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
      "kind": "LinkedField",
      "name": "organizationIndustryMainCategoriesReferences",
      "plural": true,
      "selections": [
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
          "kind": "LinkedField",
          "name": "subCategories",
          "plural": true,
          "selections": [
            (v0/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "name",
              "storageKey": null
            }
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMultipleChoicesIndustries_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationTermsOfUse_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "3fe2b9e654a2f862dc75fa3a34c2f4a1";

export default node;
