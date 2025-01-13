/**
 * @generated SignedSource<<0fafee995557cd2d2f5b3aacdbee58b3>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ReaderFragment } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationAdmin_query$data = {
  readonly organization: {
    readonly about: string | null | undefined;
    readonly canModify: boolean;
    readonly id: string;
    readonly industrySubCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
    readonly logoUrl: string | null | undefined;
    readonly name: string;
    readonly offering: {
      readonly end: any;
      readonly featureSet: ReadonlyArray<{
        readonly description: string;
        readonly name: string;
      }>;
      readonly free: boolean;
      readonly id: string;
      readonly name: string;
      readonly start: any;
      readonly unitPrice: number;
    };
    readonly website: string | null | undefined;
  } | null | undefined;
  readonly organizationBillingInfo: {
    readonly addressLine1: string | null | undefined;
    readonly addressLine2: string | null | undefined;
    readonly city: string | null | undefined;
    readonly country: string | null | undefined;
    readonly email: string | null | undefined;
    readonly id: string;
    readonly province: string | null | undefined;
    readonly suburb: string | null | undefined;
    readonly zipcode: string | null | undefined;
  } | null | undefined;
  readonly organizationIndustryMainCategoriesReferences: ReadonlyArray<{
    readonly subCategories: ReadonlyArray<{
      readonly id: string;
      readonly name: string;
    }>;
  }>;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query">;
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
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v2 = [
  (v0/*: any*/),
  (v1/*: any*/)
];
return {
  "argumentDefinitions": [
    {
      "kind": "RootArgument",
      "name": "organizationId"
    }
  ],
  "kind": "Fragment",
  "metadata": null,
  "name": "organizationAdmin_query",
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
        (v0/*: any*/),
        (v1/*: any*/),
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "logoUrl",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "about",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "website",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "canModify",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
          "kind": "LinkedField",
          "name": "industrySubCategories",
          "plural": true,
          "selections": (v2/*: any*/),
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "concreteType": "OrganizationOfferingDetails",
          "kind": "LinkedField",
          "name": "offering",
          "plural": false,
          "selections": [
            (v0/*: any*/),
            (v1/*: any*/),
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "start",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "end",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "unitPrice",
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "concreteType": "OrganizationFeatureSetDetails",
              "kind": "LinkedField",
              "name": "featureSet",
              "plural": true,
              "selections": [
                (v1/*: any*/),
                {
                  "alias": null,
                  "args": null,
                  "kind": "ScalarField",
                  "name": "description",
                  "storageKey": null
                }
              ],
              "storageKey": null
            },
            {
              "alias": null,
              "args": null,
              "kind": "ScalarField",
              "name": "free",
              "storageKey": null
            }
          ],
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
          "selections": (v2/*: any*/),
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "alias": null,
      "args": [
        {
          "kind": "Variable",
          "name": "organizationId",
          "variableName": "organizationId"
        }
      ],
      "concreteType": "OrganizationBillingInfo",
      "kind": "LinkedField",
      "name": "organizationBillingInfo",
      "plural": false,
      "selections": [
        (v0/*: any*/),
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
          "name": "addressLine1",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "addressLine2",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "suburb",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "city",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "province",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "zipcode",
          "storageKey": null
        },
        {
          "alias": null,
          "args": null,
          "kind": "ScalarField",
          "name": "country",
          "storageKey": null
        }
      ],
      "storageKey": null
    },
    {
      "args": null,
      "kind": "FragmentSpread",
      "name": "organizationMultipleChoicesIndustries_query"
    }
  ],
  "type": "Query",
  "abstractKey": null
};
})();

(node as any).hash = "c872016a55ed6d05f8494fcdfe966b59";

export default node;
