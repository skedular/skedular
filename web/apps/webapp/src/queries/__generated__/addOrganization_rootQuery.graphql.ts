/**
 * @generated SignedSource<<70edcdae04c6d13cf046501b8a5032f8>>
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
  readonly me: {
    readonly id: string;
    readonly isLocationOnboardingDone: boolean;
    readonly isOrganizationOnboardingDone: boolean;
  } | null | undefined;
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "organizationTermsOfUse_query">;
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
};
return {
  "fragment": {
    "argumentDefinitions": [],
    "kind": "Fragment",
    "metadata": null,
    "name": "addOrganization_rootQuery",
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
    "name": "addOrganization_rootQuery",
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
          (v0/*: any*/),
          (v2/*: any*/),
          {
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
          }
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "75b7735445f10f01b44acbb93979951a",
    "id": null,
    "metadata": {},
    "name": "addOrganization_rootQuery",
    "operationKind": "query",
    "text": "query addOrganization_rootQuery {\n  me {\n    id\n    isOrganizationOnboardingDone\n    isLocationOnboardingDone\n  }\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationTermsOfUse_query\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n"
  }
};
})();

(node as any).hash = "dd309398ae89f7f7421c5a3096699a13";

export default node;
