/**
 * @generated SignedSource<<2330c6e8ffb52ee78ffd66b701e80377>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberVisibilityPolicy = "FULL_ACCESS" | "LIMITED_ACCESS" | "%future added value";
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  memberVisibilityPolicy: OrganizationMemberVisibilityPolicy;
  name: string;
  termsOfUseId: string;
  type: OrganizationType;
  website?: string | null | undefined;
};
export type addOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
      readonly id: string;
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
export type addOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly contactEmail: string | null | undefined;
      readonly contactPhone: string | null | undefined;
      readonly id: string;
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
export type addOrganization_addOrganizationMutation = {
  rawResponse: addOrganization_addOrganizationMutation$rawResponse;
  response: addOrganization_addOrganizationMutation$data;
  variables: addOrganization_addOrganizationMutation$variables;
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
],
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "id",
            "storageKey": null
          },
          (v1/*: any*/),
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
            "selections": (v2/*: any*/),
            "storageKey": null
          },
          {
            "alias": null,
            "args": null,
            "concreteType": "OrganizationMemberVisibilityPolicyDetails",
            "kind": "LinkedField",
            "name": "memberVisibilityPolicy",
            "plural": false,
            "selections": (v2/*: any*/),
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
    "name": "addOrganization_addOrganizationMutation",
    "selections": (v3/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addOrganization_addOrganizationMutation",
    "selections": (v3/*: any*/)
  },
  "params": {
    "cacheID": "c9225cc15a099472c7df4783fa01cc96",
    "id": null,
    "metadata": {},
    "name": "addOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n      type {\n        type\n        name\n      }\n      memberVisibilityPolicy {\n        type\n        name\n      }\n      contactEmail\n      contactPhone\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "30694de5d24ba3307f295ee5b8a3baa9";

export default node;
