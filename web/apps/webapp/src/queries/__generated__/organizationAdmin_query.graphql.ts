/**
 * @generated SignedSource<<864ac4500997ad48a871c3327790a6ca>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_query$data = {
  readonly me: {
    readonly emails: ReadonlyArray<string>;
    readonly id: string;
    readonly preferredCustomTags: ReadonlyArray<{
      readonly id: string;
    }>;
    readonly preferredZones: ReadonlyArray<{
      readonly id: string;
    }>;
  };
  readonly organizationIndustryMainCategoriesReferences: ReadonlyArray<{
    readonly subCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "singleChoiceOrganizationMemberVisibilityPolicyquery" | "singleChoiceOrganizationType_query">;
  readonly " $fragmentType": "organizationAdmin_query";
};
export type organizationAdmin_query$key = {
  readonly " $data"?: organizationAdmin_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationAdmin_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = [
  (v0/*: any*/)
];
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationAdmin_query",
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
          "name": "emails",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "preferredZones",
          "plural": true,
          "selections": (v1/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationTagDetails",
          "kind": "LinkedField",
          "name": "preferredCustomTags",
          "plural": true,
          "selections": (v1/*: any*/),
          "storageKey": null
        }
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
      "name": "singleChoiceOrganizationType_query"
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "singleChoiceOrganizationMemberVisibilityPolicyquery"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "611fb883b1cab1f1b3b3dd94560d735e";

export default node;
