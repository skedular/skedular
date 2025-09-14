/**
 * @generated SignedSource<<555ee1a1ffb6cf30948e121b3c204bb2>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationType = "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddOrganizationInput = {
  about?: string | null | undefined;
  agreedToTermsOfUse: boolean;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  isListable: boolean;
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
      readonly isListable: boolean;
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
      readonly isListable: boolean;
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
            "name": "isListable",
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
    "cacheID": "98a90f8b9ed78bca9744d931e2a7e7c4",
    "id": null,
    "metadata": {},
    "name": "addMarketplaceOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addMarketplaceOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      uniqueAlphanumericName\n      isListable\n      name\n      about\n      website\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "9a1f3b555985d42260844a6e1cd46bca";

export default node;
