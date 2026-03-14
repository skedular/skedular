/**
 * @generated SignedSource<<a82221c0a4d243c38f93f957eb72dd9b>>
 * @lightSyntaxTransform
 * @nogrep
 */

/* tslint:disable */
/* eslint-disable */
// @ts-nocheck

import { ConcreteRequest } from 'relay-runtime';
export type OrganizationBillingCycle = "FORTNIGHTLY" | "MONTHLY" | "WEEKLY" | "%future added value";
export type OrganizationType = "INDIVIDUAL" | "MARKETPLACE" | "PRIVATE" | "%future added value";
export type AddOrganizationInput = {
  agreedToTermsOfUse: boolean;
  billingCycle: OrganizationBillingCycle;
  clientMutationId?: string | null | undefined;
  contactEmail?: string | null | undefined;
  contactPhone?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  listingMetadata?: ListingMetadataInput | null | undefined;
  marketplaceListingMetadata?: ListingMetadataInput | null | undefined;
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
export type ListingMetadataInput = {
  about?: string | null | undefined;
  includedFeatures?: ReadonlyArray<string> | null | undefined;
  subTitle?: string | null | undefined;
  title?: string | null | undefined;
};
export type addIndividualOrganization_addOrganizationMutation$variables = {
  input: AddOrganizationInput;
};
export type addIndividualOrganization_addOrganizationMutation$data = {
  readonly addOrganization: {
    readonly organization: {
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly uniqueAlphanumericName: string | null | undefined;
      readonly website: string | null | undefined;
    };
  };
};
export type addIndividualOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly uniqueAlphanumericName: string | null | undefined;
      readonly website: string | null | undefined;
    };
  };
};
export type addIndividualOrganization_addOrganizationMutation = {
  rawResponse: addIndividualOrganization_addOrganizationMutation$rawResponse;
  response: addIndividualOrganization_addOrganizationMutation$data;
  variables: addIndividualOrganization_addOrganizationMutation$variables;
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
            "concreteType": "ListingMetadata",
            "kind": "LinkedField",
            "name": "listingMetadata",
            "plural": false,
            "selections": [
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
                "name": "title",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "subTitle",
                "storageKey": null
              },
              {
                "alias": null,
                "args": null,
                "kind": "ScalarField",
                "name": "includedFeatures",
                "storageKey": null
              }
            ],
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
    "name": "addIndividualOrganization_addOrganizationMutation",
    "selections": (v1/*: any*/),
    "type": "Mutation",
    "abstractKey": null
  },
  "kind": "Request",
  "operation": {
    "argumentDefinitions": (v0/*: any*/),
    "kind": "Operation",
    "name": "addIndividualOrganization_addOrganizationMutation",
    "selections": (v1/*: any*/)
  },
  "params": {
    "cacheID": "8d4529b0dcc8aa5ded6ad3f6d8195be3",
    "id": null,
    "metadata": {},
    "name": "addIndividualOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addIndividualOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      uniqueAlphanumericName\n      name\n      listingMetadata {\n        about\n        title\n        subTitle\n        includedFeatures\n      }\n      website\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "731349249a9fb7c7f0654d041134f711";

export default node;
