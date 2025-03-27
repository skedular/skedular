/**
 * @generated SignedSource<<8e967ca470c1eeb60d41cdd1828ab54f>>
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
  readonly " $fragmentSpreads": FragmentRefs<"organizationMultipleChoicesIndustries_query" | "organizationTermsOfUse_query" | "singleChoiceOrganizationType_query">;
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
};
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
        "selections": [
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "type",
            "storageKey": null
          },
          (v1/*: any*/)
        ],
        "storageKey": null
      }
    ]
  },
  "params": {
    "cacheID": "68cfc0a1f732e202890dc92e590f8b22",
    "id": null,
    "metadata": {},
    "name": "addOrganization_rootQuery",
    "operationKind": "query",
    "text": "query addOrganization_rootQuery {\n  activeOrganizationTermsOfUse {\n    id\n  }\n  ...organizationMultipleChoicesIndustries_query\n  ...organizationTermsOfUse_query\n  ...singleChoiceOrganizationType_query\n}\n\nfragment organizationMultipleChoicesIndustries_query on Query {\n  organizationIndustryMainCategoriesReferences {\n    id\n    name\n    subCategories {\n      id\n      name\n    }\n  }\n}\n\nfragment organizationTermsOfUse_query on Query {\n  activeOrganizationTermsOfUse {\n    id\n    terms\n  }\n}\n\nfragment singleChoiceOrganizationType_query on Query {\n  organizationTypes {\n    type\n    name\n  }\n}\n"
  }
};
})();

(node as any).hash = "b65c802713228ded727f9a1d48ef41fc";

export default node;
