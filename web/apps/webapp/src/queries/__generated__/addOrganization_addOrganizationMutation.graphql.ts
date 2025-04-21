/**
 * @generated SignedSource<<b3482c92c15b37e0daf986e4cb0320e1>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationMemberVisibilityPolicy = "FullAccess" | "LimitedAccess" | "%future added value";
export type OrganizationType = "Marketplace" | "Private" | "%future added value";
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  memberVisibilityPolicy: OrganizationMemberVisibilityPolicy;
  name: string;
  physicalAddress: OrganizationAddressDetailsInput;
  termsOfUseId: string;
  type: OrganizationType;
  website?: string | null | undefined;
};
export type OrganizationAddressDetailsInput = {
  addressLine1: string;
  addressLine2?: string | null | undefined;
  city: string;
  country: string;
  province?: string | null | undefined;
  suburb: string;
  zipcode: string;
};
export type addOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      };
      readonly type: {
        readonly name: string;
        readonly type: OrganizationType;
      };
      readonly website: string | null | undefined;
    };
  } | null | undefined;
};
export type addOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
      readonly memberVisibilityPolicy: {
        readonly name: string;
        readonly type: OrganizationMemberVisibilityPolicy;
      };
      readonly name: string;
      readonly physicalAddress: {
        readonly addressLine1: string;
        readonly addressLine2: string | null | undefined;
        readonly city: string;
        readonly country: string;
        readonly province: string | null | undefined;
        readonly suburb: string;
        readonly zipcode: string;
      };
      readonly type: {
        readonly name: string;
        readonly type: OrganizationType;
      };
      readonly website: string | null | undefined;
    };
  } | null | undefined;
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
            "concreteType": "OrganizationAddressDetails",
            "kind": "LinkedField",
            "name": "physicalAddress",
            "plural": false,
            "selections": [
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
    "cacheID": "2a2c866fad4f8e5f79f3df767dee4bbc",
    "id": null,
    "metadata": {},
    "name": "addOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      name\n      about\n      website\n      type {\n        type\n        name\n      }\n      memberVisibilityPolicy {\n        type\n        name\n      }\n      physicalAddress {\n        addressLine1\n        addressLine2\n        suburb\n        city\n        province\n        zipcode\n        country\n      }\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "553212c4480087e6b45e1bb86e24339c";

export default node;
