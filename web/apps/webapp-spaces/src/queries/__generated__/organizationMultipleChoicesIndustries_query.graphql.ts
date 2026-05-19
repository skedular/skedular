/**
 * @generated SignedSource<<922b9513029d4bc9d4e711d7403bfa07>>
 * @lightSyntaxTransform
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationMultipleChoicesIndustries_query$data = {
  readonly organizationIndustryMainCategoriesReferences: ReadonlyArray<{
    readonly id: string;
    readonly name: string;
    readonly subCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
  }>;
  readonly " $fragmentType": "organizationMultipleChoicesIndustries_query";
};
export type organizationMultipleChoicesIndustries_query$key = {
  readonly " $data"?: organizationMultipleChoicesIndustries_query$data;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query">;
};

const node: ReaderFragment = (function(){
var v0 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
};
return {
  "argumentDefinitions": [],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationMultipleChoicesIndustries_query",
  "selections": [
    {
      "alias": null,
      "args": null,
      "concreteType": "OrganizationIndustryMainCategoryReferenceDetails",
      "kind": "LinkedField",
      "name": "organizationIndustryMainCategoriesReferences",
      "plural": true,
      "selections": [
        (v0/*:: as any*/),
        (v1/*:: as any*/),
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
          "kind": "LinkedField",
          "name": "subCategories",
          "plural": true,
          "selections": [
            (v0/*:: as any*/),
            (v1/*:: as any*/)
          ],
          "storageKey": null
        }
      ],
      "storageKey": null
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "47340049f7705785c71436ab583da3b7";

export default node;
