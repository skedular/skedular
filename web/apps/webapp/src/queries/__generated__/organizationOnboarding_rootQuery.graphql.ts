/**
 * @generated SignedSource<<2d114c41aff7e45b50648b3865297175>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
import { FragmentRefs } from "relay-runtime";
export type organizationOnboarding_rootQuery$variables = Record<PropertyKey, never>;
export type organizationOnboarding_rootQuery$data = {
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
};
export type organizationOnboarding_rootQuery = {
  response: organizationOnboarding_rootQuery$data;
  variables: organizationOnboarding_rootQuery$variables;
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
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = {
  "alias": null,
  "args": null,
  "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
  "kind": "LinkedField",
  "name": "subCategories",
  "plural": true,
  "selections": [
    (v0/*: any*/),
    (v2/*: any*/)
  ],
  "storageKey": null
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationOnboarding_rootQuery",
    "selections": [
      (v1/*: any*/),
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
          (v3/*: any*/)
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
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": [],
    "kind": "Operation",
    "name": "organizationOnboarding_rootQuery",
    "selections": [
      (v1/*: any*/),
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
          (v3/*: any*/),
          (v0/*: any*/),
          (v2/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "c091a346162b7ea11696d7885d5124ca",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_rootQuery",
    "operationKind": "query",
    "text": "query organizationOnboarding_rootQuery {\n  me {\n    id\n    isOrganizationOnboardingDone\n    isLocationOnboardingDone\n  }\n  activeOrganizationTermsOfUse {\n    id\n  }\n  organizationIndustryMainCategoriesReferences {\n    subCategories {\n      id\n      name\n    }\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationTermsOfUse_query\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n"
  }
};
})();

(node as any).hash = "881af125e299785dbc9d68a492e0279c";

export default node;
