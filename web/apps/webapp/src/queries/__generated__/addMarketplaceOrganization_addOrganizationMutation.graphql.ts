/**
 * @generated SignedSource<<ae6dc57e168a8650f27081830cf42d1d>>
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
  uniqueAlphanumericName?: string | null | undefined;
  website?: string | null | undefined;
};
export type addMarketplaceOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addMarketplaceOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly uniqueAlphanumericName: string | null | undefined;
      readonly website: string | null | undefined;
    };
  };
};
export type addMarketplaceOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly uniqueAlphanumericName: string | null | undefined;
      readonly website: string | null | undefined;
    };
  };
};
export type addMarketplaceOrganization_addOrganizationMutation = {
  rawResponse: addMarketplaceOrganization_addOrganizationMutation$rawResponse;
  response: addMarketplaceOrganization_addOrganizationMutation$data;
  variables: addMarketplaceOrganization_addOrganizationMutation$variables;
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "uniqueAlphanumericName",
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
            "concreteType": "OrganizationMemberVisibilityPolicyDetails",
            "kind": "LinkedField",
            "name": "memberVisibilityPolicy",
            "plural": false,
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
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "selections": (v2/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "selections": (v2/*: any*/)
  },
  "params": {
    "cacheID": "aa69284801d57eeef3237593807f247c",
    "id": null,
    "metadata": {},
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addMarketplaceOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      uniqueAlphanumericName\n      name\n      about\n      website\n      memberVisibilityPolicy {\n        type\n        name\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "15aca34e30e86fc9be9599e565c84b92";

export default node;
