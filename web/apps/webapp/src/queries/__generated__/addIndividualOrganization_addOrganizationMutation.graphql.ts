/**
 * @generated SignedSource<<63955fa6eed9b247e1e6a6936da8298f>>
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
  customDomain?: string | null | undefined;
  featureImages?: ReadonlyArray<CdnImageFileInput> | null | undefined;
  id?: string | null | undefined;
  industrySubCategoryIds: ReadonlyArray<string>;
  listingMetadata?: ListingMetadataInput | null | undefined;
  marketplaceListingMetadata?: ListingMetadataInput | null | undefined;
  name: string;
  termsOfUseId: string;
  type: OrganizationType;
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
      readonly customDomain: string | null | undefined;
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
      readonly website: string | null | undefined;
    };
  };
};
export type addIndividualOrganization_addOrganizationMutation$rawResponse = {
  readonly addOrganization: {
    readonly organization: {
      readonly customDomain: string | null | undefined;
      readonly id: string;
      readonly listingMetadata: {
        readonly about: string | null | undefined;
        readonly includedFeatures: ReadonlyArray<string> | null | undefined;
        readonly subTitle: string | null | undefined;
        readonly title: string | null | undefined;
      };
      readonly name: string;
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
            "name": "customDomain",
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
    "cacheID": "b57663620c7ab4deb2e35c59d2718f55",
    "id": null,
    "metadata": {},
    "name": "addIndividualOrganization_addOrganizationMutation",
    "operationKind": "mutation",
    "text": "mutation addIndividualOrganization_addOrganizationMutation(\n  $input: AddOrganizationInput!\n) {\n  addOrganization(input: $input) {\n    organization {\n      id\n      customDomain\n      name\n      listingMetadata {\n        about\n        title\n        subTitle\n        includedFeatures\n      }\n      website\n    }\n  }\n}\n"
  }
};
})();

(node as any).hash = "b14d8b7493a73598803b400346188866";

export default node;
