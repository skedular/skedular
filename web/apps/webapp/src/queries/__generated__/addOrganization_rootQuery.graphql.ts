/**
 * @generated SignedSource<<7cd1a82432aca6fc111bb57c02ad9887>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type addOrganization_rootQuery$variables = Record<PropertyKey, never>;
export type addOrganization_rootQuery$data = {
  readonly activeOrganizationTermsOfUse: {
    readonly id: string;
  };
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "organizationTermsOfUse_query" | "singleChoiceOrganizationMemberVisibilityPolicyquery" | "singleChoiceOrganizationType_query">;
};
export type addOrganization_rootQuery = {
  response: addOrganization_rootQuery$data;
  variables: addOrganization_rootQuery$variables;
};

const node: ConcreteRequest = (function(){
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
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v1/*: any*/)
];
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "addOrganization_rootQuery",
    "selections": [
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
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationMultipleChoicesIndustries_query"
      },
      {
        "args": null,
        "kind": "FragmentSpread",
        "name": "organizationTermsOfUse_query"
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
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "addOrganization_rootQuery",
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTermsOfUse",
        "kind": "LinkedField",
        "name": "activeOrganizationTermsOfUse",
        "plural": false,
        "selections": [
          (v0/*: any*/),
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "terms",
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
          (v0/*: any*/),
          (v1/*: any*/),
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "subCategories",
            "plural": true,
            "selections": [
              (v0/*: any*/),
              (v1/*: any*/)
            ],
            "storageKey": null
          }
        ],
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationTypeDetails",
        "kind": "LinkedField",
        "name": "organizationTypes",
        "plural": true,
        "selections": (v2/*: any*/),
        "storageKey": null
      },
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationMemberVisibilityPolicyDetails",
        "kind": "LinkedField",
        "name": "organizationMemberVisibilityPolicies",
        "plural": true,
        "selections": (v2/*: any*/),
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "388a16743f0b9c3a0aa08dfce81fb9f5",
    "id": null,
    "metadata": {},
    "name": "addOrganization_rootQuery",
    "operationKind": "query",
    "text": "query addOrganization_rootQuery {\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationTermsOfUse_query\n  ...singleChoiceOrganizationType_query\n  ...singleChoiceOrganizationMemberVisibilityPolicyquery\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n\nfragment singleChoiceOrganizationMemberVisibilityPolicyquery on Query {\n  organizationMemberVisibilityPolicies {\n    type\n    name\n  }\n}\n\nfragment singleChoiceOrganizationType_query on Query {\n  organizationTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "c8976010b86d7ad2b17a41cd1b5a4eab";

export default node;
