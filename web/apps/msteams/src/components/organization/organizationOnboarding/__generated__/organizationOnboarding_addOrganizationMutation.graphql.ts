/**
 * @generated SignedSource<<e8112d9b060d35822857e60ccf60db88>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest, Mutation } from 'relay-runtime';
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  name: string;
  termsOfUseId: string;
  website?: string | null | undefined;
};
export type organizationOnboarding_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type organizationOnboarding_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationOnboarding_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly industrySubCategories: ReadonlyArray<{
        readonly id: string;
        readonly name: string;
      }>;
      readonly name: string;
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type organizationOnboarding_addOrganizationMutation = {
  rawResponse: organizationOnboarding_addOrganizationMutation$rawResponse;
  response: organizationOnboarding_addOrganizationMutation$data;
  variables: organizationOnboarding_addOrganizationMutation$variables;
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
    "args": [
      {
        "kind": "Variable",
        "name": "input",
        "variableName": "input"
      }
    ],
    "concreteType": "OrganizationPayload",
    "kind": "LinkedField",
    "name": "addOrganization",
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
            "concreteType": "OrganizationIndustrySubCategoryReferenceDetails",
            "kind": "LinkedField",
            "name": "industrySubCategories",
            "plural": true,
            "selections": [
              (v1/*: any*/),
              (v2/*: any*/)
            ],
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
    "name": "organizationOnboarding_addOrganizationMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "organizationOnboarding_addOrganizationMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "a1e8abe93c4b7c0569b941fa424b1609",
    "id": null,
    "metadata": {},
    "name": "organizationOnboarding_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation organizationOnboarding_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n      industrySubCategories {\n        id\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "16f08b31f95c0e881d7eee98c1c36c8c";

export default node;
