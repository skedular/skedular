/**
 * @generated SignedSource<<d7c7c9edc0f625301a57c766ac836c20>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberVisibilityPolicy = "FULL_ACCESS" | "LIMITED_ACCESS" | "%future added value";
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type UpdateOrganizationInput = {
  about?: string | null | undefined;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  id: string;
  industrySubCategoryIds: ReadonlyArray<string>;
  memberVisibilityPolicy: OrganizationMemberVisibilityPolicy;
  name: string;
  type: OrganizationType;
  uniqueAlphanumericName?: string | null | undefined;
  website?: string | null | undefined;
};
export type organizationAdmin_updateOrganizationMutation$variables = {
  input: UpdateOrganizationInput;
};
export type organizationAdmin_updateOrganizationMutation$data = {
  readonly updateOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly type: {
        readonly name: string;
        readonly type: OrganizationType;
      };
      readonly website: string | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationMutation$rawResponse = {
  readonly updateOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly type: {
        readonly name: string;
        readonly type: OrganizationType;
      };
      readonly website: string | null | undefined;
    };
  };
};
export type organizationAdmin_updateOrganizationMutation = {
  rawResponse: organizationAdmin_updateOrganizationMutation$rawResponse;
  response: organizationAdmin_updateOrganizationMutation$data;
  variables: organizationAdmin_updateOrganizationMutation$variables;
};

const node: ConcreteRequest = (function(){
var v0 = [
  {
    "defaultValue": null,
    "kind": "LocalArgument",
    "name": "input"
  }
],
v1 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "id",
  "storageKey": null
},
v2 = {
  "alias": null,
  "args": null,
  "kind": "ScalarField",
  "name": "name",
  "storageKey": null
},
v3 = [
  {
    "alias": null,
    "args": null,
    "kind": "ScalarField",
    "name": "type",
    "storageKey": null
  },
  (v2/*: any*/)
],
v4 = [
  {
    "alias": null,
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "updateOrganization",
    "plural": false,
    "selections": [
      {
        "alias": null,
        "args": null,
        "concreteType": "OrganizationDetails",
        "kind": "LinkedField",
        "name": "organization",
        "plural": false,
        "selections": [
          (v1/*: any*/),
          (v2/*: any*/),
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
            "concreteType": "OrganizationTypeDetails",
            "kind": "LinkedField",
            "name": "type",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberVisibilityPolicyDetails",
            "kind": "LinkedField",
            "name": "memberVisibilityPolicy",
            "plural": false,
            "selections": (v3/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "industrySubCategories",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/)
            ],
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
          }
        ],
        "storageKey": null
      }
    ],
    "storageKey": null
  }
];
return {
  "fragment": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Fragment",
    "metadata": null,
    "name": "organizationAdmin_updateOrganizationMutation",
    "selections": (v4/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationAdmin_updateOrganizationMutation",
    "selections": (v4/*: any*/)
  },
  "params": {
    "cacheID": "aeaf341bab5714277a8644ce62349471",
    "id": null,
    "metadata": {},
    "name": "organizationAdmin_updateOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationAdmin_updateOrganizationMutation(\n  $input: UpdateOrganizationInput!\n) {\n  updateOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n      type {\n        type\n        name\n      }\n      memberVisibilityPolicy {\n        type\n        name\n      }\n      industrySubCategories {\n        id\n        name\n      }\n      contactEmail\n      contactPhone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "e1652b0797abde48015d2c72e73023ed";

export default node;
