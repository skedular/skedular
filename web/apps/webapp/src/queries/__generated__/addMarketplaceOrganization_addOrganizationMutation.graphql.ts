/**
 * @generated SignedSource<<0f4ed1fdce9ea26949fe4a4eb2369b3c>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationType = "INDIVIDUAL" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  name: string;
  termsOfUseId: string;
  type: OrganizationType;
  uniqueAlphanumericName?: string | null | undefined;
  website?: string | null | undefined;
};
export type CdnImageFileInput = {
  original?: CdnFileInput | null | undefined;
  thumbnail?: CdnFileInput | null | undefined;
};
export type CdnFileInput = {
  height?: number | null | undefined;
  url: string;
  width?: number | null | undefined;
};
export type addMarketplaceOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addMarketplaceOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly about: string | null | undefined;
      readonly id: string;
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
v1 = [
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
          {
            "alias": null,
            "args": null,
            "kind": "ScalarField",
            "name": "name",
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
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "83f7506a1a0fb32ad5bbb8039e0e1575",
    "id": null,
    "metadata": {},
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addMarketplaceOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      uniqueAlphanumericName\n      name\n      about\n      website\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "7aed7f4fd892989b72c7e9bdfc9d074b";

export default node;
